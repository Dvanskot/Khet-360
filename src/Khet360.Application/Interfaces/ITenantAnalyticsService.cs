using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
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
