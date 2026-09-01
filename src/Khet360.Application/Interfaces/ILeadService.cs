using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;

namespace Khet360.Application.Interfaces;

public interface ILeadService
{
    Task<Guid> CreateLeadAsync(LeadCreateDto leadDto, Guid branchId);
    Task<LeadDto?> GetLeadAsync(Guid id);
    Task<PagedList<LeadDto>> SearchLeadsAsync(LeadSearchFilter filter);
    Task UpdateLeadAsync(Guid id, LeadUpdateDto leadDto);
    Task<Guid> ConvertLeadAsync(Guid leadId, LeadConversionDto conversionDto);
}

public record LeadCreateDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Source,
    string Notes,
    string Industry,
    string CompanyName);

public record LeadUpdateDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Notes,
    LeadStatus Status);

public record LeadConversionDto(
    bool CreateOpportunity,
    bool CreateCustomer,
    string CustomerType, // "Individual" or "Organisation"
    decimal? EstimatedValue,
    string OpportunityName);

public record LeadDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Source,
    string Notes,
    LeadStatus Status,
    DateTime CreatedAt,
    Guid BranchId);

public record LeadSearchFilter(
    string? Query,
    LeadStatus? Status,
    Guid? BranchId,
    int Page = 1,
    int PageSize = 20);
