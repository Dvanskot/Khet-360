using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class PlatformAuditService : IPlatformAuditService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ILogger<PlatformAuditService> _logger;

    public PlatformAuditService(PlatformDbContext platformDb, ILogger<PlatformAuditService> logger)
    {
        _platformDb = platformDb;
        _logger = logger;
    }

    public async Task LogAuditAsync(
        Guid tenantId,
        string tenantSlug,
        AuditAction action,
        string entityType,
        string entityId,
        string? oldValues,
        string? newValues,
        string performedBy,
        bool isSuccess,
        string? errorMessage = null,
        string? reason = null)
    {
        var log = new TenantAuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TenantSlug = tenantSlug,
            PerformedByUsername = performedBy,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            Reason = reason,
            PerformedAtUtc = DateTime.UtcNow
        };

        _platformDb.TenantAuditLogs.Add(log);
        await _platformDb.SaveChangesAsync();

        _logger.LogInformation("Audit: {Action} {EntityType} {EntityId} by {User}. Success: {IsSuccess}", action, entityType, entityId, performedBy, isSuccess);
    }

    public async Task<List<AuditLogDto>> GetTenantAuditLogAsync(Guid tenantId, int page = 1, int pageSize = 50)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        return await _platformDb.TenantAuditLogs
            .AsNoTracking()
            .Where(l => l.TenantId == tenantId)
            .OrderByDescending(l => l.PerformedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new AuditLogDto(
                l.Id,
                l.TenantId,
                l.TenantSlug,
                l.PerformedByUsername,
                l.Action,
                l.EntityType,
                l.EntityId,
                l.IsSuccess,
                l.ErrorMessage,
                l.PerformedAtUtc,
                l.Reason))
            .ToListAsync();
    }

    public async Task<List<AuditLogDto>> GetPlatformAuditLogAsync(int page = 1, int pageSize = 50)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        return await _platformDb.TenantAuditLogs
            .AsNoTracking()
            .OrderByDescending(l => l.PerformedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new AuditLogDto(
                l.Id,
                l.TenantId,
                l.TenantSlug,
                l.PerformedByUsername,
                l.Action,
                l.EntityType,
                l.EntityId,
                l.IsSuccess,
                l.ErrorMessage,
                l.PerformedAtUtc,
                l.Reason))
            .ToListAsync();
    }
}
