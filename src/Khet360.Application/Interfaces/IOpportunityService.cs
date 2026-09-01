using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;

namespace Khet360.Application.Interfaces;

public interface IOpportunityService
{
    Task<Guid> CreateOpportunityAsync(OpportunityCreateDto opportunityDto, Guid customerId, Guid branchId);
    Task<OpportunityDto?> GetOpportunityAsync(Guid id);
    Task<PagedList<OpportunityDto>> SearchOpportunitiesAsync(OpportunitySearchFilter filter);
    Task UpdateOpportunityAsync(Guid id, OpportunityUpdateDto opportunityDto);
    Task CloseOpportunityAsync(Guid id, bool won, string notes);
}

public record OpportunityCreateDto(
    string Name,
    decimal EstimatedValue,
    DateTime ExpectedCloseDate,
    OpportunityStage Stage,
    string Notes);

public record OpportunityUpdateDto(
    string Name,
    decimal EstimatedValue,
    DateTime ExpectedCloseDate,
    OpportunityStage Stage,
    string Notes);

public record OpportunityDto(
    Guid Id,
    string Name,
    decimal EstimatedValue,
    DateTime ExpectedCloseDate,
    OpportunityStage Stage,
    Guid CustomerId,
    Guid BranchId,
    string Notes,
    DateTime CreatedAt);

public record OpportunitySearchFilter(
    string? Query,
    OpportunityStage? Stage,
    Guid? CustomerId,
    Guid? BranchId,
    int Page = 1,
    int PageSize = 20);
