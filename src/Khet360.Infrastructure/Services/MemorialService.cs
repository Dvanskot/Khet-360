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

public record MemorialDto(
    Guid Id,
    string Title,
    MemorialType Type,
    string? Theme,
    bool IsPublic,
    string? PublicUrl,
    ObituaryDto? Obituary,
    List<MemorialTributeDto> Tributes);

public record ObituaryDto(
    Guid Id,
    string Content,
    string? ImageUrl,
    ObituaryStatus Status,
    DateTime PublishedAt);

public record MemorialTributeDto(
    Guid Id,
    string AuthorName,
    string Message,
    DateTime PostedAt,
    bool IsApproved);

public record MemorialCreateDto(
    string Title,
    MemorialType Type,
    string? Theme,
    Guid FuneralCaseId);

public record ObituaryCreateDto(
    string Content,
    string? ImageUrl,
    Guid MemorialId);

public record TributeCreateDto(
    string AuthorName,
    string Message,
    Guid MemorialId);

public interface IMemorialService
{
    Task<Guid> CreateMemorialAsync(MemorialCreateDto dto, Guid branchId);
    Task<MemorialDto?> GetMemorialAsync(Guid id);
    Task CreateObituaryAsync(ObituaryCreateDto dto, Guid branchId);
    Task AddTributeAsync(TributeCreateDto dto, Guid branchId);
    Task ApproveTributeAsync(Guid tributeId);
}

public class MemorialService : IMemorialService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public MemorialService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> CreateMemorialAsync(MemorialCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var memorial = new Memorial
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Type = dto.Type,
            Theme = dto.Theme,
            FuneralCaseId = dto.FuneralCaseId,
            BranchId = branchId,
            IsPublic = false
        };

        _db.Memorials.Add(memorial);
        await _db.SaveChangesAsync();

        return memorial.Id;
    }

    public async Task<MemorialDto?> GetMemorialAsync(Guid id)
    {
        var memorial = await _db.Memorials
            .Include(m => m.Obituary)
            .Include(m => m.Tributes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (memorial == null) return null;

        return new MemorialDto(
            memorial.Id,
            memorial.Title,
            memorial.Type,
            memorial.Theme,
            memorial.IsPublic,
            memorial.PublicUrl,
            memorial.Obituary == null ? null : new ObituaryDto(
                memorial.Obituary.Id,
                memorial.Obituary.Content,
                memorial.Obituary.ImageUrl,
                memorial.Obituary.Status,
                memorial.Obituary.PublishedAt),
            memorial.Tributes.Select(t => new MemorialTributeDto(
                t.Id,
                t.AuthorName,
                t.Message,
                t.PostedAt,
                t.IsApproved)).ToList());
    }

    public async Task CreateObituaryAsync(ObituaryCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var obituary = new Obituary
        {
            Id = Guid.NewGuid(),
            Content = dto.Content,
            ImageUrl = dto.ImageUrl,
            Status = ObituaryStatus.Draft,
            PublishedAt = DateTime.UtcNow,
            MemorialId = dto.MemorialId,
            BranchId = branchId
        };

        _db.Obituaries.Add(obituary);
        await _db.SaveChangesAsync();
    }

    public async Task AddTributeAsync(TributeCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var tribute = new MemorialTribute
        {
            Id = Guid.NewGuid(),
            AuthorName = dto.AuthorName,
            Message = dto.Message,
            PostedAt = DateTime.UtcNow,
            IsApproved = false,
            MemorialId = dto.MemorialId,
            BranchId = branchId
        };

        _db.MemorialTributes.Add(tribute);
        await _db.SaveChangesAsync();
    }

    public async Task ApproveTributeAsync(Guid tributeId)
    {
        var tribute = await _db.MemorialTributes.FindAsync(tributeId);
        if (tribute == null) throw new KeyNotFoundException("Tribute not found.");

        tribute.IsApproved = true;
        await _db.SaveChangesAsync();
    }
}
