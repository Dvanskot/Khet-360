using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Domain.Events;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Khet360.Infrastructure.Services;

public class LeadService : ILeadService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;
    private readonly ICustomerService _customerService;
    private readonly IOpportunityService _opportunityService;
    private readonly ICacheService _cache;
    private readonly IMessageBus _messageBus;

    public LeadService(
        TenantDbContext tenantDb,
        ITenantService tenantService,
        ICustomerService customerService,
        IOpportunityService opportunityService,
        ICacheService cache,
        IMessageBus messageBus)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
        _customerService = customerService;
        _opportunityService = opportunityService;
        _cache = cache;
        _messageBus = messageBus;
    }

    public async Task<Guid> CreateLeadAsync(LeadCreateDto leadDto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            FirstName = leadDto.FirstName,
            LastName = leadDto.LastName,
            Email = leadDto.Email,
            Phone = leadDto.Phone,
            Source = leadDto.Source,
            Notes = leadDto.Notes,
            Industry = leadDto.Industry,
            CompanyName = leadDto.CompanyName,
            Status = LeadStatus.New,
            BranchId = branchId,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _tenantDb.Leads.Add(lead);
        await _tenantDb.SaveChangesAsync();

        return lead.Id;
    }

    public async Task<LeadDto?> GetLeadAsync(Guid id)
    {
        string cacheKey = $"lead:{id}";
        var cachedLead = await _cache.GetAsync<LeadDto>(cacheKey);
        if (cachedLead != null) return cachedLead;

        var lead = await _tenantDb.Leads.FindAsync(id);
        if (lead == null) return null;

        var dto = new LeadDto(
            lead.Id,
            lead.FirstName,
            lead.LastName,
            lead.Email,
            lead.Phone,
            lead.Source,
            lead.Notes,
            lead.Status,
            lead.CreatedAt,
            lead.BranchId);

        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));

        return dto;
    }

    public async Task<PagedList<LeadDto>> SearchLeadsAsync(LeadSearchFilter filter)
    {
        var query = _tenantDb.Leads.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(l =>
                (l.FirstName != null && l.FirstName.Contains(filter.Query)) ||
                (l.LastName != null && l.LastName.Contains(filter.Query)) ||
                (l.Email != null && l.Email.Contains(filter.Query)) ||
                (l.Phone != null && l.Phone.Contains(filter.Query)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(l => l.Status == filter.Status.Value);
        }

        if (filter.BranchId.HasValue)
        {
            query = query.Where(l => l.BranchId == filter.BranchId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedList<LeadDto>(
            items.Select(l => new LeadDto(
                l.Id,
                l.FirstName,
                l.LastName,
                l.Email,
                l.Phone,
                l.Source,
                l.Notes,
                l.Status,
                l.CreatedAt,
                l.BranchId)).ToList(),
            total,
            filter.Page,
            filter.PageSize);
    }

    public async Task UpdateLeadAsync(Guid id, LeadUpdateDto leadDto)
    {
        var lead = await _tenantDb.Leads.FindAsync(id);
        if (lead == null) throw new KeyNotFoundException("Lead not found.");

        lead.FirstName = leadDto.FirstName;
        lead.LastName = leadDto.LastName;
        lead.Email = leadDto.Email;
        lead.Phone = leadDto.Phone;
        lead.Notes = leadDto.Notes;
        lead.Status = leadDto.Status;

        await _tenantDb.SaveChangesAsync();

        await _cache.RemoveAsync($"lead:{id}");
    }

    public async Task<Guid> ConvertLeadAsync(Guid leadId, LeadConversionDto conversionDto)
    {
        var lead = await _tenantDb.Leads.FindAsync(leadId);
        if (lead == null) throw new KeyNotFoundException("Lead not found.");

        using var transaction = await _tenantDb.Database.BeginTransactionAsync();
        try
        {
            // 1. Mark Lead as Converted
            lead.Status = LeadStatus.Converted;
            _tenantDb.Leads.Update(lead);

            Guid? customerId = null;

            // 2. Create Customer if requested
            if (conversionDto.CreateCustomer)
            {
                if (conversionDto.CustomerType == "Organisation")
                {
                    customerId = await _customerService.CreateOrganisationAsync(new CreateOrganisationRequest(
                        lead.CompanyName ?? "Converted Organisation",
                        null,
                        null,
                        lead.Industry ?? "General",
                        lead.BranchId,
                        new List<AddressDto>(),
                        new List<ContactDto>()));
                }
                else
                {
                    customerId = await _customerService.CreateIndividualAsync(new CreateIndividualRequest(
                        lead.FirstName ?? "Unknown",
                        lead.LastName ?? "Unknown",
                        null,
                        null,
                        "",
                        null,
                        lead.BranchId,
                        new List<AddressDto>(),
                        new List<ContactDto>()));
                }
            }

            // 3. Create Opportunity if requested
            Guid? createdOppId = null;
            if (conversionDto.CreateOpportunity)
            {
                if (customerId == null)
                    throw new InvalidOperationException("A customer must be created or exist to link the opportunity.");

                var oppDto = new OpportunityCreateDto(
                    conversionDto.OpportunityName ?? $"Opp from Lead {lead.Id}",
                    conversionDto.EstimatedValue ?? 0,
                    DateTime.UtcNow.AddMonths(1),
                    OpportunityStage.Qualification,
                    $"Converted from Lead: {lead.FirstName} {lead.LastName}");

                createdOppId = await _opportunityService.CreateOpportunityAsync(oppDto, customerId.Value, lead.BranchId);
            }

            await _tenantDb.SaveChangesAsync();
            await transaction.CommitAsync();

            // Publish LeadConvertedEvent for async processing
            await _messageBus.PublishAsync(new LeadConvertedEvent(
                leadId,
                customerId ?? Guid.Empty,
                createdOppId,
                DateTime.UtcNow));

            return customerId ?? lead.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
