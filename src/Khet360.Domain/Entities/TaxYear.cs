using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class TaxYear : BaseEntity
{
    public string YearLabel { get; set; } = string.Empty; // e.g., "2026/2027"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
