using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a natural person as a customer.
/// </summary>
public class IndividualCustomer : Customer
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [Required, MaxLength(50)]
    public string IdentityNumber { get; set; } = null!;

    [MaxLength(50)]
    public string? IdentityType { get; set; }
}
