using System;
using System.Collections.Generic;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class InstallationJob : BaseEntity
{
    public Guid MemorialId { get; set; }
    public virtual Memorial Memorial { get; set; } = null!;

    public Guid BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public Guid? VehicleId { get; set; }
    public virtual FuneralVehicle? Vehicle { get; set; }

    public Guid? LeadArtisanId { get; set; }
    public virtual Employee? LeadArtisan { get; set; }

    public DateTime? ScheduledDate { get; set; }
    public DateTime? ActualInstallationDate { get; set; }

    public InstallationStatus Status { get; set; } = InstallationStatus.Scheduled;
    public string? InstallationNotes { get; set; }

    public virtual ICollection<InstallationChecklist> Checklist { get; set; } = new List<InstallationChecklist>();
    public virtual InstallationSignOff SignOff { get; set; } = null!;
}

public enum InstallationStatus
{
    Scheduled,
    SiteReady,
    InProgress,
    Completed,
    Cancelled,
    Delayed
}
