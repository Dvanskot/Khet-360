using System.Collections.Generic;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IDealBoardService
{
    Task<DealBoardDto> GetLeadBoardAsync(Guid branchId);
    Task<DealBoardDto> GetOpportunityBoardAsync(Guid branchId);
    Task UpdateLeadStatusAsync(Guid leadId, int newStatus);
    Task UpdateOpportunityStageAsync(Guid opportunityId, int newStage);
}
