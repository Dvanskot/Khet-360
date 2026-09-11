using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class PlatformAnnouncementService : IPlatformAnnouncementService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ILogger<PlatformAnnouncementService> _logger;

    public PlatformAnnouncementService(PlatformDbContext platformDb, ILogger<PlatformAnnouncementService> logger)
    {
        _platformDb = platformDb;
        _logger = logger;
    }

    public async Task<AnnouncementDto> CreateAnnouncementAsync(string title, string content, AnnouncementPriority priority, AnnouncementScope scope, Guid? specificTenantId, string targetPlanCategories, Guid createdByUserId, string createdByUsername)
    {
        var announcement = new PlatformAnnouncement
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            Priority = priority,
            Scope = scope,
            SpecificTenantId = specificTenantId,
            TargetPlanCategories = targetPlanCategories,
            StartDate = DateTime.UtcNow,
            IsActive = true,
            CreatedByUserId = createdByUserId,
            CreatedByUsername = createdByUsername,
            CreatedAt = DateTime.UtcNow
        };

        _platformDb.PlatformAnnouncements.Add(announcement);
        await _platformDb.SaveChangesAsync();

        return new AnnouncementDto(
            announcement.Id,
            announcement.Title,
            announcement.Content,
            announcement.Priority,
            announcement.Scope,
            announcement.IsActive,
            announcement.StartDate,
            announcement.EndDate,
            announcement.CreatedAt
        );
    }

    public async Task<List<AnnouncementDto>> GetActiveAnnouncementsAsync(Guid? tenantId = null)
    {
        var now = DateTime.UtcNow;
        var query = _platformDb.PlatformAnnouncements
            .AsNoTracking()
            .Where(a => a.IsActive && a.StartDate <= now && (!a.EndDate.HasValue || a.EndDate >= now));

        if (!tenantId.HasValue)
        {
            query = query.Where(a => a.Scope == AnnouncementScope.All);
        }
        else
        {
            var tenant = await _platformDb.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId.Value);
            if (tenant == null) return new List<AnnouncementDto>();

            query = query.Where(a =>
                a.Scope == AnnouncementScope.All ||
                (a.Scope == AnnouncementScope.SpecificTenant && a.SpecificTenantId == tenantId) ||
                (a.Scope == AnnouncementScope.PlanCategory && !string.IsNullOrEmpty(a.TargetPlanCategories) && a.TargetPlanCategories.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Contains(tenant.SubscriptionPlan.Category.ToString()))
            );
        }

        return await query
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new AnnouncementDto(
                a.Id,
                a.Title,
                a.Content,
                a.Priority,
                a.Scope,
                a.IsActive,
                a.StartDate,
                a.EndDate,
                a.CreatedAt
            ))
            .ToListAsync();
    }

    public async Task<bool> DeactivateAnnouncementAsync(Guid announcementId)
    {
        var announcement = await _platformDb.PlatformAnnouncements.FindAsync(announcementId);
        if (announcement == null) return false;

        announcement.IsActive = false;
        announcement.EndDate = DateTime.UtcNow;
        announcement.UpdatedAt = DateTime.UtcNow;

        await _platformDb.SaveChangesAsync();
        return true;
    }
}
