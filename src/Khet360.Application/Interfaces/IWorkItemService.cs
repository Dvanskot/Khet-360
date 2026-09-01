using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IWorkItemService
{
    Task<Guid> CreateWorkItemAsync(string entityType, Guid entityId, string nextAction, WorkItemPriority priority, DateTime dueDate, Guid branchId);
    Task AssignWorkItemAsync(Guid workItemId, Guid userId);
    Task UpdateStatusAsync(Guid workItemId, WorkItemStatus status, string nextAction);
    Task CompleteWorkItemAsync(Guid workItemId, string outcome);
    Task<PagedList<WorkItemDto>> GetMyWorkAsync(Guid userId, Guid branchId);
    Task<PagedList<WorkItemDto>> GetTeamQueueAsync(Guid branchId);
}

public record WorkItemDto(
    Guid Id,
    string SourceEntityType,
    Guid SourceEntityId,
    Guid? OwnerId,
    WorkItemPriority Priority,
    DateTime DueDate,
    WorkItemStatus Status,
    string? CurrentState,
    string? NextAction,
    SlaStatus SlaStatus,
    Guid BranchId);
