using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a corporate entity as a customer.
/// </summary>
public class OrganisationCustomer : Customer
{
    [Required, MaxLength(255)]
    public string OrganisationName { get; set; } = null!;

    [MaxLength(100)]
    public string? RegistrationNumber { get; set; }

    [MaxLength(100)]
    public string? TaxNumber { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    public override string FullName => OrganisationName;
}
