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

public class RepatriationService : IRepatriationService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public RepatriationService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> RequestRepatriationAsync(RepatriationCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var repatriation = new Repatriation
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = dto.ReferenceNumber,
            Status = RepatriationStatus.Requested,
            TransportMethod = dto.TransportMethod,
            OriginCountry = dto.OriginCountry,
            DestinationCountry = dto.DestinationCountry,
            RequestedAt = DateTime.UtcNow,
            Notes = dto.Notes,
            FuneralCaseId = dto.FuneralCaseId,
            BranchId = branchId
        };

        _db.Repatriations.Add(repatriation);
        await _db.SaveChangesAsync();

        return repatriation.Id;
    }

    public async Task<RepatriationDto?> GetRepatriationAsync(Guid id)
    {
        var r = await _db.Repatriations.FindAsync(id);
        if (r == null) return null;

        return new RepatriationDto(
            r.Id,
            r.ReferenceNumber,
            r.Status,
            r.TransportMethod,
            r.OriginCountry,
            r.DestinationCountry,
            r.RequestedAt,
            r.CompletedAt,
            r.Notes,
            r.FuneralCaseId);
    }

    public async Task UpdateRepatriationStatusAsync(Guid id, RepatriationUpdateDto dto)
    {
        var r = await _db.Repatriations.FindAsync(id);
        if (r == null) throw new KeyNotFoundException("Repatriation record not found.");

        r.Status = dto.Status;
        r.CompletedAt = dto.CompletedAt ?? r.CompletedAt;
        r.Notes = dto.Notes ?? r.Notes;

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<RepatriationDto>> GetRepatriationsByCaseAsync(Guid funeralCaseId)
    {
        return await _db.Repatriations
            .Where(r => r.FuneralCaseId == funeralCaseId)
            .Select(r => new RepatriationDto(
                r.Id,
                r.ReferenceNumber,
                r.Status,
                r.TransportMethod,
                r.OriginCountry,
                r.DestinationCountry,
                r.RequestedAt,
                r.CompletedAt,
                r.Notes,
                r.FuneralCaseId))
            .ToListAsync();
    }
}
