using System.Net;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Khet360.Tests;

public class SystemIntegrationTests
{
    private ILogger<T> CreateLogger<T>() => Mock.Of<ILogger<T>>();

    private IConfiguration CreateConfig(string prometheusUrl) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Prometheus:Url"] = prometheusUrl
        })
        .Build();

    private static HttpResponseMessage CreatePrometheusResponse(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private static string BuildPrometheusVectorJson(double value) => JsonSerializer.Serialize(new
    {
        status = "success",
        data = new
        {
            resultType = "vector",
            result = new[]
            {
                new { value = new[] { "1234567890", value.ToString("0.######", System.Globalization.CultureInfo.InvariantCulture) } }
            }
        }
    });

    [Fact]
    public async Task ProductivityScorecardService_Should_Parse_Prometheus_Query_Response()
    {
        var handler = new FakeHttpMessageHandler((request, cancellationToken) =>
        {
            var url = request.RequestUri?.ToString() ?? string.Empty;
            var decodedUrl = Uri.UnescapeDataString(url);
            if (decodedUrl.Contains("khet360_leads_converted_total"))
                return Task.FromResult(CreatePrometheusResponse(BuildPrometheusVectorJson(42)));
            if (decodedUrl.Contains("khet360_sla_breaches_total"))
                return Task.FromResult(CreatePrometheusResponse(BuildPrometheusVectorJson(3)));
            if (decodedUrl.Contains("khet360_case_closure_duration_seconds_sum"))
                return Task.FromResult(CreatePrometheusResponse(BuildPrometheusVectorJson(3600)));
            if (decodedUrl.Contains("khet360_case_closure_duration_seconds_count"))
                return Task.FromResult(CreatePrometheusResponse(BuildPrometheusVectorJson(1)));
            if (decodedUrl.Contains("on_time=\"true\""))
                return Task.FromResult(CreatePrometheusResponse(BuildPrometheusVectorJson(8)));
            if (decodedUrl.Contains("khet360_work_item_completion_total"))
                return Task.FromResult(CreatePrometheusResponse(BuildPrometheusVectorJson(10)));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:9090") };
        var logger = CreateLogger<ProductivityScorecardService>();

        var service = new ProductivityScorecardService(httpClient, logger);
        var scorecard = await service.GetScorecardAsync(Guid.NewGuid());

        scorecard.TotalLeadsConverted.Should().Be(42);
        scorecard.TotalSlaBreaches.Should().Be(3);
        scorecard.AverageCaseClosureTimeSeconds.Should().Be(3600);
        scorecard.SlaComplianceRate.Should().Be(0.8);
        scorecard.LeadConversionRate.Should().Be(0);
    }

    [Fact]
    public async Task ProductivityScorecardService_Should_Return_Zero_When_Prometheus_Returns_Empty()
    {
        var emptyResponse = JsonSerializer.Serialize(new
        {
            status = "success",
            data = new
            {
                resultType = "vector",
                result = Array.Empty<object>()
            }
        });

        var handler = new FakeHttpMessageHandler(CreatePrometheusResponse(emptyResponse));
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:9090") };
        var logger = CreateLogger<ProductivityScorecardService>();

        var service = new ProductivityScorecardService(httpClient, logger);
        var scorecard = await service.GetScorecardAsync(Guid.NewGuid());

        scorecard.TotalLeadsConverted.Should().Be(0);
        scorecard.TotalSlaBreaches.Should().Be(0);
        scorecard.AverageCaseClosureTimeSeconds.Should().Be(0);
        scorecard.SlaComplianceRate.Should().Be(0);
        scorecard.LeadConversionRate.Should().Be(0);
    }

    [Fact]
    public async Task ProductivityScorecardService_Should_Handle_Prometheus_Failure_Gracefully()
    {
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:9090") };
        var logger = CreateLogger<ProductivityScorecardService>();

        var service = new ProductivityScorecardService(httpClient, logger);
        var scorecard = await service.GetScorecardAsync(Guid.NewGuid());

        scorecard.TotalLeadsConverted.Should().Be(0);
        scorecard.TotalSlaBreaches.Should().Be(0);
        scorecard.AverageCaseClosureTimeSeconds.Should().Be(0);
        scorecard.SlaComplianceRate.Should().Be(0);
        scorecard.LeadConversionRate.Should().Be(0);
    }

    [Fact]
    public async Task IntelligenceService_Should_Return_Healthy_With_Metrics()
    {
        var responses = new Queue<HttpResponseMessage>();
        responses.Enqueue(CreatePrometheusResponse(BuildPrometheusVectorJson(5)));
        responses.Enqueue(CreatePrometheusResponse(BuildPrometheusVectorJson(0.00025)));

        var handler = new FakeHttpMessageHandler((request, cancellationToken) =>
        {
            if (responses.Count > 0)
                return Task.FromResult(responses.Dequeue());
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:9090") };
        var logger = CreateLogger<IntelligenceService>();

        var platformOptions = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: "PlatformDB_" + Guid.NewGuid().ToString())
            .Options;
        using var platformDb = new PlatformDbContext(platformOptions);
        platformDb.Tenants.Add(new Tenant { Id = Guid.NewGuid(), Slug = "test-tenant", Name = "Test Tenant", CreatedAt = DateTime.UtcNow });
        await platformDb.SaveChangesAsync();

        var cache = new Mock<IPlatformCacheService>();
        cache.Setup(c => c.GetTenantsAsync()).ReturnsAsync(new List<Tenant> { new Tenant { Id = Guid.NewGuid(), Slug = "test-tenant", Name = "Test Tenant", CreatedAt = DateTime.UtcNow } });

        var service = new IntelligenceService(platformDb, httpClient, cache.Object, logger);
        var health = await service.GetPlatformHealthAsync();

        health.IsHealthy.Should().BeTrue();
        health.TotalActiveTenants.Should().Be(1);
        health.TotalSlaBreachesLast24h.Should().Be(5);
        health.AverageResponseTimeMs.Should().Be(0.25);
    }

    [Fact]
    public async Task IntelligenceService_Should_Handle_Prometheus_Failure_Gracefully()
    {
        var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:9090") };
        var logger = CreateLogger<IntelligenceService>();

        var platformOptions = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: "PlatformDB_" + Guid.NewGuid().ToString())
            .Options;
        using var platformDb = new PlatformDbContext(platformOptions);
        platformDb.Tenants.Add(new Tenant { Id = Guid.NewGuid(), Slug = "test-tenant", Name = "Test Tenant", CreatedAt = DateTime.UtcNow });
        await platformDb.SaveChangesAsync();

        var cache = new Mock<IPlatformCacheService>();
        cache.Setup(c => c.GetTenantsAsync()).ReturnsAsync(new List<Tenant> { new Tenant { Id = Guid.NewGuid(), Slug = "test-tenant", Name = "Test Tenant", CreatedAt = DateTime.UtcNow } });

        var service = new IntelligenceService(platformDb, httpClient, cache.Object, logger);
        var health = await service.GetPlatformHealthAsync();

        health.IsHealthy.Should().BeTrue();
        health.TotalActiveTenants.Should().Be(1);
        health.TotalSlaBreachesLast24h.Should().Be(0);
        health.AverageResponseTimeMs.Should().Be(0);
    }

    [Fact]
    public async Task MetricsService_Should_Record_Metrics_Without_Error()
    {
        var metrics = new MetricsService();

        var exception = Record.Exception(() =>
        {
            metrics.IncrementLeadConverted();
            metrics.IncrementSlaBreach();
            metrics.RecordCaseClosureTime(3600);
            metrics.RecordWorkItemCompletion(true);
            metrics.RecordWorkItemCompletion(false);
        });

        exception.Should().BeNull();
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _sendAsync = (_, __) => Task.FromResult(response);
        }

        public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        {
            _sendAsync = sendAsync;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _sendAsync(request, cancellationToken);
        }
    }

    private class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _client;

        public FakeHttpClientFactory(HttpClient client)
        {
            _client = client;
        }

        public HttpClient CreateClient(string name) => _client;
        public HttpClient CreateClient() => _client;
    }
}
