using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class InsurancePolicyPlanItem : BaseEntity
{
    public Guid PolicyPlanId { get; set; }
    public virtual InsurancePolicyPlan PolicyPlan { get; set; } = null!;

    public Guid FuneralProductId { get; set; }
    public virtual FuneralProduct FuneralProduct { get; set; } = null!;

    public int Quantity { get; set; } = 1;
}
