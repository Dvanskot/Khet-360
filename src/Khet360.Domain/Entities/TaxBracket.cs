using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class TaxBracket : BaseEntity
{
    public Guid TaxYearId { get; set; }
    public virtual TaxYear TaxYear { get; set; } = null!;

    public decimal LowerLimit { get; set; }
    public decimal? UpperLimit { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal PercentageOverLowerLimit { get; set; }
}
