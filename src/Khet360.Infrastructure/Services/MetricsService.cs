using System;
using Khet360.Application.Interfaces;
using Prometheus;

namespace Khet360.Infrastructure.Services;

public class MetricsService : IMetricsService
{
    private static readonly Counter LeadsConvertedCounter = Metrics.CreateCounter(
        "khet360_leads_converted_total",
        "Total number of leads converted to customers");

    private static readonly Histogram CaseClosureDuration = Metrics.CreateHistogram(
        "khet360_case_closure_duration_seconds",
        "Duration of funeral case closure process");

    private static readonly Counter WorkItemCompletionCounter = Metrics.CreateCounter(
        "khet360_work_item_completion_total",
        "Total work items completed",
        new CounterConfiguration { LabelNames = new[] { "on_time" } });

    private static readonly Counter SlaBreachCounter = Metrics.CreateCounter(
        "khet360_sla_breaches_total",
        "Total number of SLA breaches");

    public void IncrementLeadConverted()
    {
        LeadsConvertedCounter.Inc();
    }

    public void RecordCaseClosureTime(double seconds)
    {
        CaseClosureDuration.Observe(seconds);
    }

    public void RecordWorkItemCompletion(bool onTime)
    {
        WorkItemCompletionCounter.WithLabels(onTime ? "true" : "false").Inc();
    }

    public void IncrementSlaBreach()
    {
        SlaBreachCounter.Inc();
    }
}
