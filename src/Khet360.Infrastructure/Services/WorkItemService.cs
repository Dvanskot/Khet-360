using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Khet360.Infrastructure.Services;

public class WorkItemService : IWorkItemService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;
    private readonly IRoutingService _routingService;

    public WorkItemService(TenantDbContext tenantDb, ITenantService tenantService, IRoutingService routingService)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
        _routingService = routingService;
    }

    public async Task<Guid> CreateWorkItemAsync(string entityType, Guid entityId, string nextAction, WorkItemPriority priority, DateTime dueDate, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        var workItem = new WorkItem
        {
            Id = Guid.NewGuid(),
            SourceEntityType = entityType,
            SourceEntityId = entityId,
            NextAction = nextAction,
            Priority = priority,
            DueDate = dueDate,
            Status = WorkItemStatus.Pending,
            BranchId = branchId,
            CreatedAt = DateTime.UtcNow
        };

        _tenantDb.WorkItems.Add(workItem);
        await _tenantDb.SaveChangesAsync();

        // Intelligent Routing: Attempt to auto-assign to the best available user based on entity type and branch
        var bestUserId = await _routingService.FindBestUserAsync(entityType, branchId);
        if (bestUserId.HasValue)
        {
            await AssignWorkItemAsync(workItem.Id, bestUserId.Value);
        }

        return workItem.Id;
    }

    public async Task AssignWorkItemAsync(Guid workItemId, Guid userId)
    {
        var workItem = await _tenantDb.WorkItems.FindAsync(workItemId);
        if (workItem == null) throw new KeyNotFoundException("Work item not found.");

        workItem.OwnerId = userId;
        workItem.UpdatedAt = DateTime.UtcNow;

        _tenantDb.WorkItemHistories.Add(new WorkItemHistory
        {
            Id = Guid.NewGuid(),
            WorkItemId = workItemId,
            NewOwnerId = userId,
            OldOwnerId = workItem.OwnerId,
            TimestampUtc = DateTime.UtcNow,
            Note = "Assigned to user"
        });

        await _tenantDb.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid workItemId, WorkItemStatus status, string nextAction)
    {
        var workItem = await _tenantDb.WorkItems.FindAsync(workItemId);
        if (workItem == null) throw new KeyNotFoundException("Work item not found.");

        var oldStatus = workItem.Status;
        workItem.Status = status;
        workItem.NextAction = nextAction;
        workItem.UpdatedAt = DateTime.UtcNow;

        _tenantDb.WorkItemHistories.Add(new WorkItemHistory
        {
            Id = Guid.NewGuid(),
            WorkItemId = workItemId,
            OldStatus = oldStatus.ToString(),
            NewStatus = status.ToString(),
            TimestampUtc = DateTime.UtcNow,
            Note = "Status updated"
        });

        await _tenantDb.SaveChangesAsync();
    }

    public async Task CompleteWorkItemAsync(Guid workItemId, string outcome)
    {
        await UpdateStatusAsync(workItemId, WorkItemStatus.Completed, "None");

        var workItem = await _tenantDb.WorkItems.FindAsync(workItemId);
        // Log the outcome in history
        _tenantDb.WorkItemHistories.Add(new WorkItemHistory
        {
            Id = Guid.NewGuid(),
            WorkItemId = workItemId,
            Note = $"Completed with outcome: {outcome}",
            TimestampUtc = DateTime.UtcNow
        });

        await _tenantDb.SaveChangesAsync();
    }

    public async Task<PagedList<WorkItemDto>> GetMyWorkAsync(Guid userId, Guid branchId)
    {
        var query = _tenantDb.WorkItems
            .Where(wi => wi.OwnerId == userId && wi.Status != WorkItemStatus.Completed && wi.BranchId == branchId);

        var total = await query.CountAsync();
        var items = await query.ToListAsync();

        return new PagedList<WorkItemDto>(
            items.Select(MapToDto).ToList(),
            total,
            1, 100);
    }

    public async Task<PagedList<WorkItemDto>> GetTeamQueueAsync(Guid branchId)
    {
        var query = _tenantDb.WorkItems
            .Where(wi => wi.OwnerId == null && wi.Status != WorkItemStatus.Completed && wi.BranchId == branchId);

        var total = await query.CountAsync();
        var items = await query.ToListAsync();

        return new PagedList<WorkItemDto>(
            items.Select(MapToDto).ToList(),
            total,
            1, 100);
    }

    private WorkItemDto MapToDto(WorkItem wi)
    {
        var slaStatus = CalculateSlaStatus(wi);

        return new WorkItemDto(
            wi.Id,
            wi.SourceEntityType,
            wi.SourceEntityId,
            wi.OwnerId,
            wi.Priority,
            wi.DueDate,
            wi.Status,
            wi.CurrentState,
            wi.NextAction,
            slaStatus,
            wi.BranchId);
    }

    private SlaStatus CalculateSlaStatus(WorkItem wi)
    {
        if (wi.Status == WorkItemStatus.Completed || wi.Status == WorkItemStatus.Cancelled)
            return SlaStatus.OnTrack;

        var now = DateTime.UtcNow;
        if (now > wi.DueDate)
            return SlaStatus.Breached;

        var timeUntilDue = wi.DueDate - now;

        // Warning if due within 24 hours for Medium/Low, or 4 hours for High/Critical
        var warningThreshold = wi.Priority switch
        {
            WorkItemPriority.Critical => TimeSpan.FromHours(4),
            WorkItemPriority.High => TimeSpan.FromHours(8),
            WorkItemPriority.Medium => TimeSpan.FromHours(24),
            _ => TimeSpan.FromHours(48)
        };

        if (timeUntilDue <= warningThreshold)
            return SlaStatus.Warning;

        return SlaStatus.OnTrack;
    }
}
