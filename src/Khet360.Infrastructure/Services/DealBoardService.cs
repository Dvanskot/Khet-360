using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class DealBoardService : IDealBoardService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;
    private readonly IStateSyncService _stateSync;

    public DealBoardService(TenantDbContext db, ITenantService tenantService, IStateSyncService stateSync)
    {
        _db = db;
        _tenantService = tenantService;
        _stateSync = stateSync;
    }

    public async Task<DealBoardDto> GetLeadBoardAsync(Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var leads = await _db.Leads
            .Where(l => l.BranchId == branchId)
            .ToListAsync();

        var columns = Enum.GetValues(typeof(LeadStatus))
            .Cast<LeadStatus>()
            .Select(status => new DealColumnDto(
                status.ToString(),
                (int)status,
                leads.Where(l => l.Status == status)
                    .Select(l => new DealCardDto(
                        l.Id,
                        $"{l.FirstName} {l.LastName}",
                        l.Source,
                        null,
                        l.CreatedAt,
                        l.Status.ToString()))
                    .ToList()))
            .ToList();

        return new DealBoardDto("Lead Pipeline", columns);
    }

    public async Task<DealBoardDto> GetOpportunityBoardAsync(Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var opportunities = await _db.Opportunities
            .Where(o => o.BranchId == branchId)
            .ToListAsync();

        var columns = Enum.GetValues(typeof(OpportunityStage))
            .Cast<OpportunityStage>()
            .Select(stage => new DealColumnDto(
                stage.ToString(),
                (int)stage,
                opportunities.Where(o => o.Stage == stage)
                    .Select(o => new DealCardDto(
                        o.Id,
                        o.Name,
                        o.Notes ?? "No notes",
                        o.EstimatedValue,
                        o.CreatedAt,
                        o.Stage.ToString()))
                    .ToList()))
            .ToList();

        return new DealBoardDto("Opportunity Pipeline", columns);
    }

    public async Task UpdateLeadStatusAsync(Guid leadId, int newStatus)
    {
        var lead = await _db.Leads.FindAsync(leadId);
        if (lead == null) throw new KeyNotFoundException("Lead not found.");

        lead.Status = (LeadStatus)newStatus;
        await _db.SaveChangesAsync();

        await _stateSync.NotifyStateChangedAsync("Lead", leadId, lead.BranchId, "StatusUpdated");
    }

    public async Task UpdateOpportunityStageAsync(Guid opportunityId, int newStage)
    {
        var opp = await _db.Opportunities.FindAsync(opportunityId);
        if (opp == null) throw new KeyNotFoundException("Opportunity not found.");

        opp.Stage = (OpportunityStage)newStage;
        await _db.SaveChangesAsync();

        await _stateSync.NotifyStateChangedAsync("Opportunity", opportunityId, opp.BranchId, "StageUpdated");
    }
}
