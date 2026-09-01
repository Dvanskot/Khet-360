using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IFamilyRelationshipService
{
    Task AddRelationshipAsync(Guid fromCustomerId, Guid toCustomerId, RelationshipType type);
    Task TerminateRelationshipAsync(Guid relationshipId, DateTime effectiveDate);
    Task<List<RelationshipDto>> GetRelationshipsForCustomerAsync(Guid customerId);
    Task<FamilyGraphDto> GetFamilyGraphAsync(Guid customerId);
}

public record RelationshipDto(
    Guid Id,
    Guid FromCustomerId,
    Guid ToCustomerId,
    RelationshipType Type,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsActive);

public record FamilyGraphDto(
    Guid RootCustomerId,
    List<RelationshipDto> Relationships);
