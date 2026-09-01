using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;

namespace Khet360.Application.Interfaces;

public interface IActivityService
{
    Task<Guid> CreateActivityAsync(ActivityCreateDto activityDto, Guid branchId);
    Task<ActivityDto?> GetActivityAsync(Guid id);
    Task UpdateActivityAsync(Guid id, ActivityUpdateDto activityDto);
    Task CompleteActivityAsync(Guid id, string outcome, string notes);
    Task<PagedList<ActivityDto>> SearchActivitiesAsync(ActivitySearchFilter filter);
}

public record ActivityCreateDto(
    string Subject,
    string Description,
    ActivityType Type,
    DateTime ScheduledAt,
    Guid? RelatedCustomerId,
    Guid? RelatedLeadId,
    Guid? RelatedOpportunityId,
    Guid? RelatedCaseId);

public record ActivityUpdateDto(
    string Subject,
    string Description,
    DateTime ScheduledAt,
    ActivityStatus Status);

public record ActivityDto(
    Guid Id,
    string Subject,
    string Description,
    ActivityType Type,
    ActivityStatus Status,
    DateTime ScheduledAt,
    DateTime? CompletedAt,
    Guid? RelatedCustomerId,
    Guid? RelatedLeadId,
    Guid? RelatedOpportunityId,
    Guid? RelatedCaseId,
    Guid BranchId);

public record ActivitySearchFilter(
    string? Query,
    ActivityStatus? Status,
    ActivityType? Type,
    Guid? BranchId,
    int Page = 1,
    int PageSize = 20);
