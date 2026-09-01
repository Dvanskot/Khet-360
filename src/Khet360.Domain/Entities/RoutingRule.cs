using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Maps a specific type of work item to the role required to handle it.
/// Used by the Intelligent Routing service for automatic assignment.
/// </summary>
public class RoutingRule
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string SourceEntityType { get; set; } = null!; // e.g., "Lead", "FuneralCase"

    [Required]
    public string RequiredRole { get; set; } = null!; // e.g., "SalesAgent", "FuneralDirector"

    [Required]
    public Guid TenantId { get; set; }

    public bool IsActive { get; set; } = true;
}
