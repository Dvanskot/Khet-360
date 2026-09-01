using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a granular permission within the system.
/// </summary>
public class Permission
{
    [Key]
    public string Code { get; set; } = null!; // e.g., "FuneralCase.Create", "Finance.ViewReports"

    [Required]
    public string Name { get; set; } = null!; // e.g., "Create Funeral Case", "View Financial Reports"

    [Required]
    public string Description { get; set; } = null!;
}
