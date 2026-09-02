using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Khet360.Infrastructure.Services;

public class ProductivityScorecardService : IProductivityScorecardService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductivityScorecardService> _logger;
    private readonly string _prometheusUrl;

    public ProductivityScorecardService(HttpClient httpClient, IConfiguration configuration, ILogger<ProductivityScorecardService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _prometheusUrl = configuration["Prometheus:Url"] ?? "http://localhost:9090";
    }

    public async Task<ProductivityScorecardDto> GetScorecardAsync(Guid branchId)
    {
        try
        {
            // Note: In a real multi-tenant setup, we would filter by labels like branch_id if the metrics were labeled.
            // Since current metrics are global, we return global stats.

            var leadsConverted = await QueryPrometheus<double>("sum(khet360_leads_converted_total)");
            var slaBreaches = await QueryPrometheus<double>("sum(khet360_sla_breaches_total)");

            var closureSum = await QueryPrometheus<double>("sum(khet360_case_closure_duration_seconds_sum)");
            var closureCount = await QueryPrometheus<double>("sum(khet360_case_closure_duration_seconds_count)");
            var avgClosureTime = closureCount > 0 ? closureSum / closureCount : 0;

            var onTimeCount = await QueryPrometheus<double>("sum(khet360_work_item_completion_total{on_time=\"true\"})");
            var totalCompleted = await QueryPrometheus<double>("sum(khet360_work_item_completion_total)");
            var complianceRate = totalCompleted > 0 ? onTimeCount / totalCompleted : 0;

            // Lead conversion rate would typically require knowing total leads.
            // We'll simulate it or use a placeholder if total leads metric doesn't exist.
            var conversionRate = 0.0; // In a full implementation, we'd query total leads

            return new ProductivityScorecardDto(
                (long)leadsConverted,
                avgClosureTime,
                (long)slaBreaches,
                complianceRate,
                conversionRate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying Prometheus for productivity scorecard.");
            return new ProductivityScorecardDto(0, 0, 0, 0, 0);
        }
    }

    private async Task<double> QueryPrometheus<T>(string query)
    {
        var response = await _httpClient.GetAsync($"/api/v1/query?query={Uri.EscapeDataString(query)}");
        response.EnsureSuccessStatusCode();

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
