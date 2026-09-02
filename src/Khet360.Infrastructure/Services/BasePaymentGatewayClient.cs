using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Khet360.Infrastructure.Services;

public abstract class BasePaymentGatewayClient
{
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILogger _logger;

    protected BasePaymentGatewayClient(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected async Task<TResponse> SendRequestAsync<TRequest, TResponse>(
        string clientName,
        HttpMethod method,
        string endpoint,
        TRequest requestBody,
        CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(clientName);

        try
        {
            _logger.LogInformation("Sending {Method} request to {Endpoint}", method, endpoint);

            var response = await client.SendAsync(new HttpRequestMessage(method, endpoint)
            {
                Content = JsonContent.Create(requestBody)
            }, ct);

            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Payment Gateway Error: {StatusCode} - {Content}", response.StatusCode, content);
                throw new HttpRequestException($"Gateway returned {response.StatusCode}: {content}");
            }

            return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during gateway request to {Endpoint}", endpoint);
            throw;
        }
    }
}
