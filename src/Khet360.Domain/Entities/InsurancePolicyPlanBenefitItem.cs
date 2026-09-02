using System;
using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Defines a specific product or service provided as part of an insurance plan benefit.
/// </summary>
public class InsurancePolicyPlanBenefitItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid BenefitId { get; set; }
    public virtual InsurancePolicyPlanBenefit Benefit { get; set; } = null!;

    [Required]
    public Guid FuneralProductId { get; set; }
    public virtual FuneralProduct FuneralProduct { get; set; } = null!;

    [Required]
    public int Quantity { get; set; } = 1;
}
