using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;

namespace Khet360.Application.Interfaces;

public record MortuarySlotDto(
    Guid Id,
    string SlotNumber,
    bool IsColdStorage,
    MortuarySlotStatus Status,
    Guid? FuneralCaseId);

public record SlotAssignmentDto(
    Guid SlotId,
    Guid FuneralCaseId);

public record SlotReleaseDto(
    Guid SlotId);

public interface IMortuaryService
{
    Task<Guid> CreateSlotAsync(string slotNumber, bool isColdStorage, Guid branchId);
    Task<IEnumerable<MortuarySlotDto>> GetAvailableSlotsAsync(Guid branchId, bool requireColdStorage = false);
    Task AssignSlotAsync(SlotAssignmentDto dto);
    Task ReleaseSlotAsync(SlotReleaseDto dto);
    Task<MortuarySlotDto?> GetSlotAsync(Guid id);
}
