using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class StatutoryRate : BaseEntity
{
    public Guid TaxYearId { get; set; }
    public virtual TaxYear TaxYear { get; set; } = null!;

    public string RateName { get; set; } = string.Empty; // e.g., "UIF_Employee", "UIF_Employer", "SDL_Employer"
    public decimal Percentage { get; set; }
    public decimal? CappingLimit { get; set; }
}
