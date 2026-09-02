using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Base entity for all customers (Individuals and Organisations).
/// Implements IBranchScoped to ensure data isolation per branch.
/// </summary>
public abstract class Customer : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;

    public string? CommunicationPreferences { get; set; } // JSON blob for flexible prefs
    public string? ConsentMetadata { get; set; } // JSON blob for regulatory consent


    [Required]
    public Guid BranchId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public abstract string FullName { get; }

    public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();
}
