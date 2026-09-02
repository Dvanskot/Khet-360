using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class ProductionLog : BaseEntity
{
    public Guid ProductionOrderId { get; set; }
    public virtual ProductionOrder ProductionOrder { get; set; } = null!;

    public ProductionStage Stage { get; set; }
    public Guid ArtisanId { get; set; }
    public virtual Employee Artisan { get; set; } = null!;

    public DateTime StartedAtUtc { get; set; }
    public DateTime EndedAtUtc { get; set; }
    public double DurationHours { get; set; }
    public string? Notes { get; set; }
}
