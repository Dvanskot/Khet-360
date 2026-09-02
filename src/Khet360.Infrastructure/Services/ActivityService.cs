using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Khet360.Infrastructure.Services;

public class ActivityService : IActivityService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;

    public ActivityService(TenantDbContext tenantDb, ITenantService tenantService)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
    }

    public async Task<Guid> CreateActivityAsync(ActivityCreateDto activityDto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            Subject = activityDto.Subject,
            Description = activityDto.Description,
            Type = activityDto.Type,
            ScheduledDate = activityDto.ScheduledAt,
            CustomerId = activityDto.RelatedCustomerId,
            LeadId = activityDto.RelatedLeadId,
            OpportunityId = activityDto.RelatedOpportunityId,
            WorkItemId = activityDto.RelatedCaseId, // CaseId is mapped to WorkItemId as per current domain
            BranchId = branchId,
            Status = ActivityStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _tenantDb.Activities.Add(activity);
        await _tenantDb.SaveChangesAsync();

        return activity.Id;
    }

    public async Task<ActivityDto?> GetActivityAsync(Guid id)
    {
        var activity = await _tenantDb.Activities.FindAsync(id);
        if (activity == null) return null;

        return new ActivityDto(
            activity.Id,
            activity.Subject,
            activity.Description ?? "",
            activity.Type,
            activity.Status,
            activity.ScheduledDate,
            activity.CompletedDate,
            activity.CustomerId,
            activity.LeadId,
            activity.OpportunityId,
            activity.WorkItemId,
            activity.BranchId);
    }

    public async Task UpdateActivityAsync(Guid id, ActivityUpdateDto activityDto)
    {
        var activity = await _tenantDb.Activities.FindAsync(id);
        if (activity == null) throw new KeyNotFoundException("Activity not found.");

        activity.Subject = activityDto.Subject;
        activity.Description = activityDto.Description;
        activity.ScheduledDate = activityDto.ScheduledAt;
        activity.Status = activityDto.Status;

        await _tenantDb.SaveChangesAsync();
    }

    public async Task CompleteActivityAsync(Guid id, string outcome, string notes)
    {
        var activity = await _tenantDb.Activities.FindAsync(id);
        if (activity == null) throw new KeyNotFoundException("Activity not found.");

        activity.Status = ActivityStatus.Completed;
        activity.CompletedDate = DateTime.UtcNow;
        activity.Description += $"\n\nOutcome: {outcome}\nNotes: {notes}";

        await _tenantDb.SaveChangesAsync();
    }

    public async Task<PagedList<ActivityDto>> SearchActivitiesAsync(ActivitySearchFilter filter)
    {
        var query = _tenantDb.Activities.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(a => (a.Subject != null && a.Subject.Contains(filter.Query)) || (a.Description != null && a.Description.Contains(filter.Query)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(a => a.Status == filter.Status.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(a => a.Type == filter.Type.Value);
        }

        if (filter.BranchId.HasValue)
        {
            query = query.Where(a => a.BranchId == filter.BranchId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedList<ActivityDto>(
            items.Select(a => new ActivityDto(
                a.Id,
                a.Subject,
                a.Description ?? "",
                a.Type,
                a.Status,
                a.ScheduledDate,
                a.CompletedDate,
                a.CustomerId,
                a.LeadId,
                a.OpportunityId,
                a.WorkItemId,
                a.BranchId)).ToList(),
            total,
            filter.Page,
            filter.PageSize);
    }
}
