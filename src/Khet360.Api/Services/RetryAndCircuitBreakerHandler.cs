using System.Net.Http;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Khet360.Api.Services;

public class RetryAndCircuitBreakerHandler : DelegatingHandler
{
    private readonly ILogger<RetryAndCircuitBreakerHandler> _logger;
    private readonly CircuitBreakerState _circuitBreaker;

    private const int MaxRetries = 3;
    private static readonly TimeSpan[] RetryDelays = new[]
    {
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromMilliseconds(1000),
        TimeSpan.FromMilliseconds(2000)
    };

    public RetryAndCircuitBreakerHandler(ILogger<RetryAndCircuitBreakerHandler> logger)
    {
        _logger = logger;
        _circuitBreaker = new CircuitBreakerState(
            failureThreshold: 5,
            timeout: TimeSpan.FromSeconds(30));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _circuitBreaker.ExecuteAsync(async ct =>
        {
            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    var response = await base.SendAsync(request, ct);
                    if (response.IsSuccessStatusCode || attempt == MaxRetries)
                    {
                        _circuitBreaker.OnSuccess();
                        return response;
                    }

                    _logger.LogWarning("Request to {Url} returned {StatusCode}. Retrying (attempt {Attempt}).",
                        request.RequestUri, response.StatusCode, attempt + 1);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Request to {Url} failed with HttpRequestException. Retrying (attempt {Attempt}).",
                        request.RequestUri, attempt + 1);
                }
                catch (TaskCanceledException ex) when (ex.InnerException != null)
                {
                    _logger.LogWarning(ex, "Request to {Url} timed out. Retrying (attempt {Attempt}).",
                        request.RequestUri, attempt + 1);
                }

                if (attempt < MaxRetries)
                {
                    await Task.Delay(RetryDelays[attempt], ct);
                }
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable);
        }, cancellationToken);
    }

    private class CircuitBreakerState
    {
        private readonly int _failureThreshold;
        private readonly TimeSpan _timeout;
        private int _failureCount;
        private DateTime _lastFailureTime;
        private bool _isClosed = true;
        private readonly object _lock = new();

        public CircuitBreakerState(int failureThreshold, TimeSpan timeout)
        {
            _failureThreshold = failureThreshold;
            _timeout = timeout;
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                if (!_isClosed && DateTime.UtcNow < _lastFailureTime + _timeout)
                {
                    throw new InvalidOperationException("Circuit breaker is open.");
                }
            }

            try
            {
                var result = await action(cancellationToken);
                return result;
            }
            catch (Exception)
            {
                lock (_lock)
                {
                    _failureCount++;
                    _lastFailureTime = DateTime.UtcNow;
                    if (_failureCount >= _failureThreshold)
                    {
                        _isClosed = false;
                    }
                }
                throw;
            }
        }

        public void OnSuccess()
        {
            lock (_lock)
            {
                _failureCount = 0;
                _isClosed = true;
            }
        }
    }
}
