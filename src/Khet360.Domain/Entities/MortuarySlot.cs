using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class MortuarySlot : IBranchScoped
{
    public Guid Id { get; set; }
    public string SlotNumber { get; set; } = string.Empty;
    public bool IsColdStorage { get; set; }
    public MortuarySlotStatus Status { get; set; }

    public Guid? FuneralCaseId { get; set; }
    public virtual FuneralCase? FuneralCase { get; set; }

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
