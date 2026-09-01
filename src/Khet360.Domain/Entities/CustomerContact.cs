using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a contact method associated with a customer.
/// </summary>
public class CustomerContact
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    [Required]
    public ContactType Type { get; set; }

    [Required, MaxLength(255)]
    public string Value { get; set; } = null!;

    public bool IsPrimary { get; set; }
}
