using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Infrastructure.Services;

public class RoutingService : IRoutingService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;

    public RoutingService(TenantDbContext tenantDb, ITenantService tenantService)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
    }

    public async Task<Guid?> FindBestUserAsync(string entityType, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        // 1. Identify required role for this entity type
        var rule = await _tenantDb.RoutingRules
            .FirstOrDefaultAsync(r => r.SourceEntityType == entityType && r.IsActive && r.TenantId == tenantId);

        if (rule == null)
        {
            // No routing rule defined, cannot auto-assign
            return null;
        }

        // 2. Find eligible users in the branch with the required role
        var eligibleUserIds = await (from ur in _tenantDb.UserRoles
                                    join u in _tenantDb.Users on ur.UserId equals u.Id
                                    join ub in _tenantDb.UserBranches on u.Id equals ub.UserId
                                    join r in _tenantDb.Roles on ur.RoleId equals r.Id
                                    where r.Name == rule.RequiredRole
                                    && ub.BranchId == branchId
                                    && u.IsActive
                                    select u.Id).ToListAsync();

        if (!eligibleUserIds.Any())
            return null;

        // 3. Calculate current load for each eligible user
        var userLoads = await _tenantDb.WorkItems
            .Where(wi => wi.OwnerId != null && eligibleUserIds.Contains(wi.OwnerId.Value)
                         && wi.Status != WorkItemStatus.Completed
                         && wi.Status != WorkItemStatus.Cancelled)
            .GroupBy(wi => wi.OwnerId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToListAsync();

        // 4. Find the user with the minimum load who is below their MaxCapacity
        var bestUserId = await _tenantDb.Users
            .Where(u => eligibleUserIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.MaxCapacity,
                Load = userLoads.Where(l => l.UserId == u.Id).Select(l => l.Count).FirstOrDefault()
            })
            .Where(x => x.Load < x.MaxCapacity)
            .OrderBy(x => x.Load)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        return bestUserId == Guid.Empty ? null : bestUserId;
    }
}
