using System;
using System.Collections.Generic;
using Khet360.Domain.Common;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class InsurancePolicyPlan : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PremiumAmount { get; set; }
    public decimal CoverBenefitAmount { get; set; }
    public int MaxMembers { get; set; } = 10;
    public Dictionary<string, object> BenefitDetails { get; set; } = new();
    public Dictionary<string, object>? AgeBandRules { get; set; }
    public int WaitingPeriodMonths { get; set; } = 6;
    public InsuranceCoverType CoverType { get; set; }

    public virtual ICollection<InsurancePolicyPlanBenefit> Benefits { get; set; } = new List<InsurancePolicyPlanBenefit>();
    public virtual ICollection<InsurancePolicyPlanItem> PlanItems { get; set; } = new List<InsurancePolicyPlanItem>();
}
