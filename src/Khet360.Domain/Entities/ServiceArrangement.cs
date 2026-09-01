using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class ServiceArrangement : IBranchScoped
{
    public Guid Id { get; set; }
    public string ArrangementName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public ArrangementType Type { get; set; }
    public string? Description { get; set; }

    // Catering details
    public bool HasCatering { get; set; }
    public int ExpectedGuestCount { get; set; }
    public string? CateringNotes { get; set; }
    public CateringStatus CateringStatus { get; set; }

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }

    public virtual ICollection<ArrangementItem> Items { get; set; } = new List<ArrangementItem>();
}
