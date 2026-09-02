using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Defines a specific benefit for a member role and age band within an insurance policy plan.
/// </summary>
public class InsurancePolicyPlanBenefit
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PolicyPlanId { get; set; }
    public virtual InsurancePolicyPlan PolicyPlan { get; set; } = null!;

    [Required]
    public MemberRole Role { get; set; }

    [Required]
    public int MinAge { get; set; } = 0;

    [Required]
    public int MaxAge { get; set; } = 120;

    [Required]
    public decimal CoverAmount { get; set; }

    [Required]
    public bool IsFixed { get; set; } = false;

    public virtual ICollection<InsurancePolicyPlanBenefitItem> BenefitItems { get; set; } = new List<InsurancePolicyPlanBenefitItem>();
}
