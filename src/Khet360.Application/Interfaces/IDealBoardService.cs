using System.Collections.Generic;
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
