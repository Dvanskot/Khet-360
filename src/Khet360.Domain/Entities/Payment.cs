using System;
using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

public class Payment : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty; // e.g., "Netcash", "BankTransfer"

    public Guid InvoiceId { get; set; }
    public virtual Invoice Invoice { get; set; } = null!;

    public Guid BranchId { get; set; }
}
