using System;
using System.Collections.Generic;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class FinancialTransaction : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public Guid SourceEntityId { get; set; } // Link to Invoice, Payment, PayrollRun, etc.
    public string SourceEntityType { get; set; } = string.Empty; // "Invoice", "Payment", "Payroll"

    public virtual ICollection<FinancialEntry> Entries { get; set; } = new List<FinancialEntry>();
}
