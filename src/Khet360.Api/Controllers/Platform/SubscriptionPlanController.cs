using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Khet360.Api.Controllers.Platform;

[ApiController]
[Route("platform/api/[controller]")]
[Authorize(AuthenticationSchemes = "PlatformJwt", Roles = "PlatformAdmin")]
public class SubscriptionPlanController : ControllerBase
{
    private readonly PlatformDbContext _platformDb;
    private readonly IPlatformCacheService _cache;

    public SubscriptionPlanController(PlatformDbContext platformDb, IPlatformCacheService cache)
    {
        _platformDb = platformDb;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionPlan>>> GetPlans()
    {
        var plans = await _cache.GetSubscriptionPlansAsync();
        return Ok(plans);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionPlan>> CreatePlan([FromBody] SubscriptionPlan plan)
    {
        plan.Id = Guid.NewGuid();
        plan.CreatedAt = DateTime.UtcNow;
        _platformDb.SubscriptionPlans.Add(plan);
        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateSubscriptionPlansAsync();
        return CreatedAtAction(nameof(GetPlans), new { id = plan.Id }, plan);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] SubscriptionPlan plan)
    {
        var existingPlan = await _platformDb.SubscriptionPlans.FindAsync(id);
        if (existingPlan == null) return NotFound();

        existingPlan.Name = plan.Name;
        existingPlan.Description = plan.Description;
        existingPlan.Category = plan.Category;
        existingPlan.MonthlyPrice = plan.MonthlyPrice;
        existingPlan.AnnualPrice = plan.AnnualPrice;
        existingPlan.TrialPeriodDays = plan.TrialPeriodDays;
        existingPlan.IsActive = plan.IsActive;
        existingPlan.UpdatedAt = DateTime.UtcNow;

        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateSubscriptionPlansAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlan(Guid id)
    {
        var plan = await _platformDb.SubscriptionPlans.FindAsync(id);
        if (plan == null) return NotFound();

        _platformDb.SubscriptionPlans.Remove(plan);
        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateSubscriptionPlansAsync();
        return NoContent();
    }

    [HttpPatch("tenants/{tenantId}/subscription")]
    public async Task<IActionResult> UpdateTenantSubscription(Guid tenantId, [FromBody] Guid planId)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) return NotFound("Tenant not found");

        var plan = await _platformDb.SubscriptionPlans.FindAsync(planId);
        if (plan == null) return NotFound("Plan not found");

        tenant.SubscriptionPlanId = planId;
        tenant.UpdatedAt = DateTime.UtcNow;
        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateTenantsAsync();

        return Ok(new { Message = "Tenant subscription updated successfully" });
    }
}
