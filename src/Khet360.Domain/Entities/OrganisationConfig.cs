using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

public class OrganisationConfig
{
    public Guid Id { get; set; }

    [Required, MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? TradingName { get; set; }

    [MaxLength(100)]
    public string RegistrationNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string VatNumber { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? PrimaryContactPerson { get; set; }

    [Required, EmailAddress, MaxLength(255)]
    public string ContactEmail { get; set; } = string.Empty;

    [MaxLength(50)]
    public string ContactPhone { get; set; } = string.Empty;

    public string LogoUrl { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#08AFAF"; // Default to ERP Teal
    public string SecondaryColor { get; set; } = "#FFFFFF";

    [MaxLength(3)]
    public string BaseCurrency { get; set; } = "ZAR";

    public string? TaxSettings { get; set; } // JSON blob for tax rules

    public int FiscalYearStartMonth { get; set; } = 1; // January

    public string DocumentNumberingPrefix { get; set; } = "KHT";

    public Tenant Tenant { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
