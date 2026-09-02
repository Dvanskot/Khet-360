using System;
using System.Collections.Generic;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class PayProfile : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string TaxBracket { get; set; } = "Standard";
}
