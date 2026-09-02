using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public enum RebateType
{
    Primary,
    Secondary,
    Tertiary
}

public class TaxRebate : BaseEntity
{
    public Guid TaxYearId { get; set; }
    public virtual TaxYear TaxYear { get; set; } = null!;

    public RebateType Type { get; set; }
    public decimal Amount { get; set; }
    public int MinAge { get; set; }
}
