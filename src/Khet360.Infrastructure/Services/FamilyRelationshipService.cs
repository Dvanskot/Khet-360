using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class FamilyRelationshipService : IFamilyRelationshipService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;

    public FamilyRelationshipService(TenantDbContext tenantDb, ITenantService tenantService)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
    }

    public async Task AddRelationshipAsync(Guid fromCustomerId, Guid toCustomerId, RelationshipType type)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        // Validate that both customers exist and belong to the same tenant
        var fromCustomer = await _tenantDb.Customers.FindAsync(fromCustomerId);
        var toCustomer = await _tenantDb.Customers.FindAsync(toCustomerId);

        if (fromCustomer == null || toCustomer == null)
            throw new KeyNotFoundException("One or both customers not found.");

        // Handle temporal auditing: Terminate any existing active relationship of the same type
        var existingActive = await _tenantDb.FamilyRelationships
            .FirstOrDefaultAsync(r => r.FromCustomerId == fromCustomerId &&
                                      r.ToCustomerId == toCustomerId &&
                                      r.Type == type &&
                                      r.IsActive);

        if (existingActive != null)
        {
            existingActive.IsActive = false;
            existingActive.ValidTo = DateTime.UtcNow;
        }

        var relationship = new FamilyRelationship
        {
            Id = Guid.NewGuid(),
            FromCustomerId = fromCustomerId,
            ToCustomerId = toCustomerId,
            Type = type,
            ValidFrom = DateTime.UtcNow,
            IsActive = true,
            BranchId = fromCustomer.BranchId // Defaults to the subject's branch
        };

        _tenantDb.FamilyRelationships.Add(relationship);
        await _tenantDb.SaveChangesAsync();
    }

    public async Task TerminateRelationshipAsync(Guid relationshipId, DateTime effectiveDate)
    {
        var relationship = await _tenantDb.FamilyRelationships.FindAsync(relationshipId);
        if (relationship == null) throw new KeyNotFoundException("Relationship not found.");

        relationship.IsActive = false;
        relationship.ValidTo = effectiveDate;

        await _tenantDb.SaveChangesAsync();
    }

    public async Task<List<RelationshipDto>> GetRelationshipsForCustomerAsync(Guid customerId)
    {
        var relationships = await _tenantDb.FamilyRelationships
            .Where(r => r.FromCustomerId == customerId || r.ToCustomerId == customerId)
            .ToListAsync();

        return relationships.Select(r => new RelationshipDto(
            r.Id,
            r.FromCustomerId,
            r.ToCustomerId,
            r.Type,
            r.ValidFrom,
            r.ValidTo,
            r.IsActive
        )).ToList();
    }

    public async Task<FamilyGraphDto> GetFamilyGraphAsync(Guid customerId)
    {
        var relationships = await _tenantDb.FamilyRelationships
            .Where(r => r.FromCustomerId == customerId || r.ToCustomerId == customerId)
            .ToListAsync();

        return new FamilyGraphDto(
            customerId,
            relationships.Select(r => new RelationshipDto(
                r.Id,
                r.FromCustomerId,
                r.ToCustomerId,
                r.Type,
                r.ValidFrom,
                r.ValidTo,
                r.IsActive
            )).ToList()
        );
    }
}
