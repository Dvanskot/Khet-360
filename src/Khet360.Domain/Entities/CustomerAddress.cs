using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a physical address associated with a customer.
/// </summary>
public class CustomerAddress
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    [Required]
    public AddressType Type { get; set; }

    [Required, MaxLength(255)]
    public string AddressLine1 { get; set; } = null!;

    [MaxLength(255)]
    public string? AddressLine2 { get; set; }

    [Required, MaxLength(100)]
    public string City { get; set; } = null!;

    [MaxLength(100)]
    public string? Province { get; set; }

    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Country { get; set; } = null!;

    public bool IsPrimary { get; set; }
}
