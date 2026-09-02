using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Khet360.Infrastructure.Services;

public class OpportunityService : IOpportunityService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;

    public OpportunityService(TenantDbContext tenantDb, ITenantService tenantService)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
    }

    public async Task<Guid> CreateOpportunityAsync(OpportunityCreateDto opportunityDto, Guid customerId, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        var opportunity = new Opportunity
        {
            Id = Guid.NewGuid(),
            Name = opportunityDto.Name,
            EstimatedValue = opportunityDto.EstimatedValue,
            ExpectedCloseDate = opportunityDto.ExpectedCloseDate,
            Stage = opportunityDto.Stage,
            CustomerId = customerId,
            BranchId = branchId,
            Notes = opportunityDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _tenantDb.Opportunities.Add(opportunity);
        await _tenantDb.SaveChangesAsync();

        return opportunity.Id;
    }

    public async Task<OpportunityDto?> GetOpportunityAsync(Guid id)
    {
        var opp = await _tenantDb.Opportunities.FindAsync(id);
        if (opp == null) return null;

        return new OpportunityDto(
            opp.Id,
            opp.Name,
            opp.EstimatedValue,
            opp.ExpectedCloseDate,
            opp.Stage,
            opp.CustomerId,
            opp.BranchId,
            opp.Notes ?? string.Empty,
            opp.CreatedAt);
    }

    public async Task<PagedList<OpportunityDto>> SearchOpportunitiesAsync(OpportunitySearchFilter filter)
    {
        var query = _tenantDb.Opportunities.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(o => o.Name.Contains(filter.Query));
        }

        if (filter.Stage.HasValue)
        {
            query = query.Where(o => o.Stage == filter.Stage.Value);
        }

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == filter.CustomerId.Value);
        }

        if (filter.BranchId.HasValue)
        {
            query = query.Where(o => o.BranchId == filter.BranchId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedList<OpportunityDto>(
            items.Select(o => new OpportunityDto(
                o.Id,
                o.Name,
                o.EstimatedValue,
                o.ExpectedCloseDate,
                o.Stage,
                o.CustomerId,
                o.BranchId,
                o.Notes ?? string.Empty,
                o.CreatedAt)).ToList(),
            total,
            filter.Page,
            filter.PageSize);
    }

    public async Task UpdateOpportunityAsync(Guid id, OpportunityUpdateDto opportunityDto)
    {
        var opp = await _tenantDb.Opportunities.FindAsync(id);
        if (opp == null) throw new KeyNotFoundException("Opportunity not found.");

        opp.Name = opportunityDto.Name;
        opp.EstimatedValue = opportunityDto.EstimatedValue;
        opp.ExpectedCloseDate = opportunityDto.ExpectedCloseDate;
        opp.Stage = opportunityDto.Stage;
        opp.Notes = opportunityDto.Notes;

        await _tenantDb.SaveChangesAsync();
    }

    public async Task CloseOpportunityAsync(Guid id, bool won, string notes)
    {
        var opp = await _tenantDb.Opportunities.FindAsync(id);
        if (opp == null) throw new KeyNotFoundException("Opportunity not found.");

        opp.Stage = won ? OpportunityStage.ClosedWon : OpportunityStage.ClosedLost;
        opp.Notes += $"\nClosed on {DateTime.UtcNow}: {(won ? "Won" : "Lost")}. {notes}";

        await _tenantDb.SaveChangesAsync();
    }
}
