using System;
using System.Collections.Generic;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class ProductionOrder : BaseEntity
{
    public Guid MemorialId { get; set; }
    public virtual Memorial Memorial { get; set; } = null!;

    public ProductionStage CurrentStage { get; set; } = ProductionStage.OrderConfirmed;
    public ProductionStatus Status { get; set; } = ProductionStatus.InProgress;
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }

    public virtual ICollection<ProductionLog> Logs { get; set; } = new List<ProductionLog>();
    public virtual ICollection<QualityCheck> QualityChecks { get; set; } = new List<QualityCheck>();
}

public enum ProductionStage
{
    OrderConfirmed,
    SlabSelection,
    CuttingShaping,
    Polishing,
    Engraving,
    Finishing,
    QualityCheck,
    ReadyForDelivery
}

public enum ProductionStatus
{
    InProgress,
    OnHold,
    Completed,
    Rejected
}
