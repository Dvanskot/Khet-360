using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities.Platform;

public class PlatformPaymentReceipt
{
    public Guid Id { get; set; }
    [Required, MaxLength(200)]
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
}
