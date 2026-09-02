using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface ITenantAnalyticsService
{
    Task<OperationalEfficiencyDto> GetOperationalEfficiencyAsync();
    Task<List<BranchPerformanceDto>> GetBranchPerformanceAsync();
    Task<SlaComplianceDto> GetSlaComplianceAsync();
    Task<WorkloadDistributionDto> GetWorkloadDistributionAsync();
}
