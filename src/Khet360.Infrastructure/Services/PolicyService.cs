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

public class PolicyService : IPolicyService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public PolicyService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> CreatePolicyAsync(PolicyCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var policy = new InsurancePolicy
        {
            Id = Guid.NewGuid(),
            PolicyNumber = dto.PolicyNumber,
            ProviderName = dto.ProviderName,
            CoverageAmount = dto.CoverageAmount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = PolicyStatus.Active,
            CustomerId = dto.CustomerId,
            BranchId = branchId
        };

        _db.InsurancePolicies.Add(policy);
        await _db.SaveChangesAsync();

        return policy.Id;
    }

    public async Task<PolicyDto?> GetPolicyAsync(Guid id)
    {
        var policy = await _db.InsurancePolicies.FindAsync(id);
        if (policy == null) return null;

        return new PolicyDto(
            policy.Id,
            policy.PolicyNumber,
            policy.ProviderName,
            policy.CoverageAmount,
            policy.StartDate,
            policy.EndDate,
            policy.Status,
            policy.CustomerId);
    }

    public async Task<IEnumerable<PolicyDto>> GetPoliciesByCustomerAsync(Guid customerId)
    {
        return await _db.InsurancePolicies
            .Where(p => p.CustomerId == customerId)
            .Select(p => new PolicyDto(
                p.Id,
                p.PolicyNumber,
                p.ProviderName,
                p.CoverageAmount,
                p.StartDate,
                p.EndDate,
                p.Status,
                p.CustomerId))
            .ToListAsync();
    }

    public async Task UpdatePolicyAsync(Guid id, PolicyUpdateDto dto)
    {
        var policy = await _db.InsurancePolicies.FindAsync(id);
        if (policy == null) throw new KeyNotFoundException("Policy not found.");

        policy.PolicyNumber = dto.PolicyNumber;
        policy.CoverageAmount = dto.CoverageAmount;
        policy.EndDate = dto.EndDate;
        policy.Status = dto.Status;

        await _db.SaveChangesAsync();
    }

    public async Task DeletePolicyAsync(Guid id)
    {
        var policy = await _db.InsurancePolicies.FindAsync(id);
        if (policy == null) throw new KeyNotFoundException("Policy not found.");

        _db.InsurancePolicies.Remove(policy);
        await _db.SaveChangesAsync();
    }
}
