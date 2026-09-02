using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class FinancialEntry : BaseEntity
{
    public Guid FinancialTransactionId { get; set; }
    public virtual FinancialTransaction FinancialTransaction { get; set; } = null!;

    public string AccountCode { get; set; } = string.Empty; // Chart of Accounts code
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
