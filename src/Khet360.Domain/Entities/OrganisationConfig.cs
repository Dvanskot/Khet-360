using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

public class OrganisationConfig
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;

    public string RegistrationNumber { get; set; } = string.Empty;
    public string VatNumber { get; set; } = string.Empty;

    public string LogoUrl { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#000000";
    public string SecondaryColor { get; set; } = "#FFFFFF";

    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;

    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
