using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class InboxMessage : BaseEntity
{
    public string MessageId { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }
}
