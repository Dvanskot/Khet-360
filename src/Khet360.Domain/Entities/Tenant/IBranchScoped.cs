namespace Khet360.Domain.Entities.Tenant;

/// <summary>
/// Interface for entities that belong to a specific branch.
/// Entities implementing this interface are subject to Branch Scoping filters.
/// </summary>
public interface IBranchScoped
{
    Guid BranchId { get; set; }
}
