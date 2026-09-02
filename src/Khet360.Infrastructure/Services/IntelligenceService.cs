using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class IntelligenceService : IIntelligenceService
{
    private readonly PlatformDbContext _platformDb;
    private readonly HttpClient _httpClient;
    private readonly ILogger<IntelligenceService> _logger;
    private readonly string _prometheusUrl;

    public IntelligenceService(PlatformDbContext platformDb, IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<IntelligenceService> logger)
    {
        _platformDb = platformDb;
        _logger = logger;
        _prometheusUrl = configuration["Prometheus:Url"] ?? "http://localhost:9090";
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<PlatformHealthDto> GetPlatformHealthAsync()
    {
        try
        {
            var totalTenants = await _platformDb.Tenants.CountAsync();
            var slaBreaches = await QueryPrometheus<double>("sum(khet360_sla_breaches_total)");
            var avgResponseTime = await QueryPrometheus<double>("avg(http_request_duration_seconds)");

            return new PlatformHealthDto(
                IsHealthy: true,
                AverageResponseTimeMs: avgResponseTime * 1000,
                TotalActiveTenants: totalTenants,
                TotalSlaBreachesLast24h: (long)slaBreaches,
                StatusMessage: "All systems operational."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating platform health.");
            return new PlatformHealthDto(false, 0, 0, 0, ex.Message);
        }
    }

    public async Task<List<TenantGrowthDto>> GetTenantGrowthTrendsAsync(DateTime from, DateTime to)
    {
        try
        {
            var tenants = await _platformDb.Tenants
                .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

            var growth = new List<TenantGrowthDto>();
            var grouped = tenants.GroupBy(t => t.CreatedAt.Date).OrderBy(g => g.Key);

            int runningTotal = await _platformDb.Tenants.CountAsync(t => t.CreatedAt < from);

            foreach (var group in grouped)
            {
                int newTenants = group.Count();
                runningTotal += newTenants;
                growth.Add(new TenantGrowthDto(
                    group.Key,
                    newTenants,
                    runningTotal,
                    0 // Growth rate calculation would require previous day's total
                ));
            }

            return growth;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating tenant growth trends.");
            return new List<TenantGrowthDto>();
        }
    }

    public async Task<List<FeatureUsageDto>> GetGlobalFeatureUsageAsync()
    {
        try
        {
            // In a real app, we'd have a list of tracked features
            var trackedFeatures = new[] { "LeadConversion", "SlaEscalation", "BackupRequest", "WizardCompletion" };
            var usage = new List<FeatureUsageDto>();

            foreach (var feature in trackedFeatures)
            {
                var count = await QueryPrometheus<double>($"sum(khet360_{feature.ToLower()}_total)");
                usage.Add(new FeatureUsageDto(
                    feature,
                    (long)count,
                    0, // Avg daily usage would need a time-series query
                    0 // % of tenants would need to cross-reference with total tenants
                ));
            }

            return usage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating global feature usage.");
            return new List<FeatureUsageDto>();
        }
    }

    private async Task<double> QueryPrometheus<T>(string query)
    {
        var response = await _httpClient.GetAsync($"/api/v1/query?query={Uri.EscapeDataString(query)}");
        if (!response.IsSuccessStatusCode) return 0;

        var result = await response.Content.ReadFromJsonAsync<PrometheusResponse>();
        var vector = result?.Data?.Result?.FirstOrDefault();

        if (vector != null && double.TryParse(vector.Value[1], out var val))
        {
            return val;
        }

        return 0;
    }

    private class PrometheusResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public PrometheusData Data { get; set; } = new();
    }

    private class PrometheusData
    {
        [JsonPropertyName("resultType")]
        public string ResultType { get; set; } = string.Empty;
        [JsonPropertyName("result")]
        public List<PrometheusResult> Result { get; set; } = new();
    }

    private class PrometheusResult
    {
        [JsonPropertyName("metric")]
        public Dictionary<string, string> Metric { get; set; } = new();
        [JsonPropertyName("value")]
        public List<string> Value { get; set; } = new();
    }
}
