using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class ServiceArrangementService : IServiceArrangementService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public ServiceArrangementService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> CreateArrangementAsync(ServiceArrangementCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var arrangement = new ServiceArrangement
        {
            Id = Guid.NewGuid(),
            ArrangementName = dto.ArrangementName,
            ScheduledDate = dto.ScheduledDate,
            Location = dto.Location,
            Type = dto.Type,
            Description = dto.Description,
            HasCatering = dto.HasCatering,
            ExpectedGuestCount = dto.ExpectedGuestCount,
            CateringNotes = dto.CateringNotes,
            CateringStatus = CateringStatus.Pending,
            FuneralCaseId = dto.FuneralCaseId,
            TenantId = tenantId,
            BranchId = branchId
        };

        if (dto.Items != null)
        {
            foreach (var itemDto in dto.Items)
            {
                arrangement.Items.Add(new ArrangementItem
                {
                    Id = Guid.NewGuid(),
                    ItemName = itemDto.ItemName,
                    Description = itemDto.Description,
                    UnitPrice = itemDto.UnitPrice,
                    Quantity = itemDto.Quantity,
                    IsProvidedByFamily = itemDto.IsProvidedByFamily,
                    TenantId = tenantId,
                    BranchId = branchId
                });
            }
        }

        _db.ServiceArrangements.Add(arrangement);
        await _db.SaveChangesAsync();

        return arrangement.Id;
    }

    public async Task<ServiceArrangementDto?> GetArrangementAsync(Guid id)
    {
        var arrangement = await _db.ServiceArrangements
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (arrangement == null) return null;

        return MapToDto(arrangement);
    }

    public async Task UpdateArrangementAsync(Guid id, ServiceArrangementUpdateDto dto)
    {
        var arrangement = await _db.ServiceArrangements.FindAsync(id);
        if (arrangement == null) throw new KeyNotFoundException("Arrangement not found.");

        arrangement.ArrangementName = dto.ArrangementName;
        arrangement.ScheduledDate = dto.ScheduledDate;
        arrangement.Location = dto.Location;
        arrangement.Type = dto.Type;
        arrangement.Description = dto.Description;
        arrangement.HasCatering = dto.HasCatering;
        arrangement.ExpectedGuestCount = dto.ExpectedGuestCount;
        arrangement.CateringNotes = dto.CateringNotes;
        arrangement.CateringStatus = dto.CateringStatus;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteArrangementAsync(Guid id)
    {
        var arrangement = await _db.ServiceArrangements.FindAsync(id);
        if (arrangement == null) throw new KeyNotFoundException("Arrangement not found.");

        _db.ServiceArrangements.Remove(arrangement);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ServiceArrangementDto>> GetArrangementsByCaseAsync(Guid funeralCaseId)
    {
        var arrangements = await _db.ServiceArrangements
            .Include(s => s.Items)
            .Where(s => s.FuneralCaseId == funeralCaseId)
            .ToListAsync();

        return arrangements.Select(MapToDto);
    }

    private ServiceArrangementDto MapToDto(ServiceArrangement s)
    {
        return new ServiceArrangementDto(
            s.Id,
            s.ArrangementName,
            s.ScheduledDate,
            s.Location,
            s.Type,
            s.Description,
            s.HasCatering,
            s.ExpectedGuestCount,
            s.CateringNotes,
            s.CateringStatus,
            s.FuneralCaseId,
            s.Items.Select(i => new ArrangementItemDto(
                i.Id,
                i.ItemName,
                i.Description,
                i.UnitPrice,
                i.Quantity,
                i.IsProvidedByFamily)).ToList());
    }
}
