using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Khet360.Infrastructure.Services;

public class FuneralCaseService : IFuneralCaseService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;
    private readonly IWorkItemService _workItemService;

    public FuneralCaseService(TenantDbContext tenantDb, ITenantService tenantService, IWorkItemService workItemService)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
        _workItemService = workItemService;
    }

    public async Task<Guid> OpenCaseAsync(Guid customerId, Guid? deceasedId, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        var caseNumber = $"KHT-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var funeralCase = new FuneralCase
        {
            Id = Guid.NewGuid(),
            CaseNumber = caseNumber,
            Status = FuneralCaseStatus.Enquiry,
            CustomerId = customerId,
            DeceasedCustomerId = deceasedId,
            BranchId = branchId,
            OpenedAt = DateTime.UtcNow
        };

        _tenantDb.FuneralCases.Add(funeralCase);
        await _tenantDb.SaveChangesAsync();

        // Trigger the first WorkItem for the Enquiry phase
        await _workItemService.CreateWorkItemAsync(
            "FuneralCase",
            funeralCase.Id,
            "Initial Case Enquiry & Verification",
            WorkItemPriority.Medium,
            DateTime.UtcNow.AddDays(2),
            branchId);

        return funeralCase.Id;
    }

    public async Task CompleteMilestoneAsync(Guid caseId, FuneralCaseStatus milestone, string outcome, string notes, Guid userId)
    {
        var funeralCase = await _tenantDb.FuneralCases
            .FirstOrDefaultAsync(c => c.Id == caseId);

        if (funeralCase == null) throw new KeyNotFoundException("Funeral case not found.");

        // 1. Log the completed milestone
        var milestoneRecord = new FuneralCaseMilestone
        {
            Id = Guid.NewGuid(),
            CaseId = caseId,
            MilestoneStatus = milestone,
            CompletedAt = DateTime.UtcNow,
            CompletedByUserId = userId,
            Outcome = outcome,
            Notes = notes
        };

        _tenantDb.FuneralCaseMilestones.Add(milestoneRecord);

        // 2. Update the case status to the next stage in the workflow
        // Sequential transition: Enquiry -> Opened -> Verification -> ...
        int nextStatusValue = (int)milestone + 1;
        if (nextStatusValue <= (int)FuneralCaseStatus.Closed)
        {
            funeralCase.Status = (FuneralCaseStatus)nextStatusValue;
        }
        else
        {
            funeralCase.Status = FuneralCaseStatus.Closed;
        }

        if (funeralCase.Status == FuneralCaseStatus.Closed)
        {
            funeralCase.ClosedAt = DateTime.UtcNow;
        }

        await _tenantDb.SaveChangesAsync();

        // 3. Trigger the WorkItem for the NEXT stage
        if (funeralCase.Status != FuneralCaseStatus.Closed)
        {
            await _workItemService.CreateWorkItemAsync(
                "FuneralCase",
                funeralCase.Id,
                $"Proceed to {funeralCase.Status} phase",
                WorkItemPriority.Medium,
                DateTime.UtcNow.AddDays(3),
                funeralCase.BranchId);
        }
    }

    public async Task<FuneralCaseDto?> GetCaseDetailsAsync(Guid id)
    {
        var funeralCase = await _tenantDb.FuneralCases
            .Include(c => c.Milestones)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (funeralCase == null) return null;

        return new FuneralCaseDto(
            funeralCase.Id,
            funeralCase.CaseNumber,
            funeralCase.Status,
            funeralCase.CustomerId,
            funeralCase.DeceasedCustomerId,
            funeralCase.OpenedAt,
            funeralCase.ClosedAt,
            funeralCase.Notes,
            funeralCase.BranchId,
            funeralCase.Milestones.Select(m => new FuneralCaseMilestoneDto(
                m.Id,
                m.MilestoneStatus,
                m.CompletedAt,
                m.CompletedByUserId,
                m.Outcome,
                m.Notes
            )).ToList()
        );
    }

    public async Task<PagedList<FuneralCaseDto>> SearchCasesAsync(FuneralCaseSearchFilter filter)
    {
        var query = _tenantDb.FuneralCases.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(c => c.CaseNumber.Contains(filter.Query));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(c => c.Status == filter.Status.Value);
        }

        if (filter.BranchId.HasValue)
        {
            query = query.Where(c => c.BranchId == filter.BranchId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedList<FuneralCaseDto>(
            items.Select(c => new FuneralCaseDto(
                c.Id,
                c.CaseNumber,
                c.Status,
                c.CustomerId,
                c.DeceasedCustomerId,
                c.OpenedAt,
                c.ClosedAt,
                c.Notes,
                c.BranchId,
                new List<FuneralCaseMilestoneDto>() // Milestones loaded separately for performance
            )).ToList(),
            total,
            filter.Page,
            filter.PageSize);
    }
}
