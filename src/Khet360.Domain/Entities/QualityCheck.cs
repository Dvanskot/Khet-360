using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class QualityCheck : BaseEntity
{
    public Guid ProductionOrderId { get; set; }
    public virtual ProductionOrder ProductionOrder { get; set; } = null!;

    public ProductionStage Stage { get; set; }
    public bool Passed { get; set; }
    public string? Comments { get; set; }
    public Guid InspectorId { get; set; }
    public virtual Employee Inspector { get; set; } = null!;
    public DateTime CheckedAtUtc { get; set; } = DateTime.UtcNow;
}
