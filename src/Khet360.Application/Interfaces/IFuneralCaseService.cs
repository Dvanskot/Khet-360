using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IFuneralCaseService
{
    Task<Guid> OpenCaseAsync(Guid customerId, Guid? deceasedId, Guid branchId);
    Task CompleteMilestoneAsync(Guid caseId, FuneralCaseStatus milestone, string outcome, string notes, Guid userId);
    Task<FuneralCaseDto?> GetCaseDetailsAsync(Guid id);
    Task<PagedList<FuneralCaseDto>> SearchCasesAsync(FuneralCaseSearchFilter filter);
}

public record FuneralCaseDto(
    Guid Id,
    string CaseNumber,
    FuneralCaseStatus Status,
    Guid CustomerId,
    Guid? DeceasedCustomerId,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    string? Notes,
    Guid BranchId,
    List<FuneralCaseMilestoneDto> Milestones);

public record FuneralCaseMilestoneDto(
    Guid Id,
    FuneralCaseStatus MilestoneStatus,
    DateTime CompletedAt,
    Guid CompletedByUserId,
    string? Outcome,
    string? Notes);

public record FuneralCaseSearchFilter(string? Query, FuneralCaseStatus? Status, Guid? BranchId, int Page = 1, int PageSize = 20);
