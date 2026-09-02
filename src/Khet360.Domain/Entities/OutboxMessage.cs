using System;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    [Required]
    public string EventType { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty; // JSON payload

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    public string? Error { get; set; }
}
