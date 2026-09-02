using System;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class PaymentConfiguration : BaseEntity
{
    [Required]
    public PaymentProvider Provider { get; set; } = PaymentProvider.Netcash;

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    public string? SecretKey { get; set; }

    [Required]
    public string MerchantId { get; set; } = string.Empty;

    public bool IsSandbox { get; set; } = true;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
