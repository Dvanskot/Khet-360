using System;

namespace Khet360.Application.Interfaces;

public interface IMetricsService
{
    void IncrementLeadConverted();
    void RecordCaseClosureTime(double seconds);
    void RecordWorkItemCompletion(bool onTime);
    void IncrementSlaBreach();
}
