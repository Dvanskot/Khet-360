using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class MortuaryService : IMortuaryService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public MortuaryService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> CreateSlotAsync(string slotNumber, bool isColdStorage, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var slot = new MortuarySlot
        {
            Id = Guid.NewGuid(),
            SlotNumber = slotNumber,
            IsColdStorage = isColdStorage,
            Status = MortuarySlotStatus.Available,
            BranchId = branchId
        };

        _db.MortuarySlots.Add(slot);
        await _db.SaveChangesAsync();

        return slot.Id;
    }

    public async Task<IEnumerable<MortuarySlotDto>> GetAvailableSlotsAsync(Guid branchId, bool requireColdStorage = false)
    {
        var query = _db.MortuarySlots
            .Where(s => s.BranchId == branchId && s.Status == MortuarySlotStatus.Available);

        if (requireColdStorage)
        {
            query = query.Where(s => s.IsColdStorage);
        }

        return await query
            .Select(s => new MortuarySlotDto(s.Id, s.SlotNumber, s.IsColdStorage, s.Status, s.FuneralCaseId))
            .ToListAsync();
    }

    public async Task AssignSlotAsync(SlotAssignmentDto dto)
    {
        var slot = await _db.MortuarySlots.FindAsync(dto.SlotId);
        if (slot == null) throw new KeyNotFoundException("Slot not found.");
        if (slot.Status != MortuarySlotStatus.Available) throw new InvalidOperationException("Slot is not available.");

        slot.Status = MortuarySlotStatus.Occupied;
        slot.FuneralCaseId = dto.FuneralCaseId;

        await _db.SaveChangesAsync();
    }

    public async Task ReleaseSlotAsync(SlotReleaseDto dto)
    {
        var slot = await _db.MortuarySlots.FindAsync(dto.SlotId);
        if (slot == null) throw new KeyNotFoundException("Slot not found.");

        slot.Status = MortuarySlotStatus.Available;
        slot.FuneralCaseId = null;

        await _db.SaveChangesAsync();
    }

    public async Task<MortuarySlotDto?> GetSlotAsync(Guid id)
    {
        var s = await _db.MortuarySlots.FindAsync(id);
        if (s == null) return null;

        return new MortuarySlotDto(s.Id, s.SlotNumber, s.IsColdStorage, s.Status, s.FuneralCaseId);
    }
}
