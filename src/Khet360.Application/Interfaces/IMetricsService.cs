using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Interfaces;

public interface IMetricsService
{
    void IncrementLeadConverted();
    void RecordCaseClosureTime(double seconds);
    void RecordWorkItemCompletion(bool onTime);
    void IncrementSlaBreach();
}
