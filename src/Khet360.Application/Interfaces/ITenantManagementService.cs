using System;
using System.Threading.Tasks;
using Khet360.Domain.Entities;

namespace Khet360.Application.Interfaces;

public interface ITenantManagementService
{
    Task<Tenant> CreateTenantAsync(string name, string slug, Guid subscriptionPlanId, IsolationTier tier);
    Task<bool> ActivateTenantAsync(Guid tenantId);
    Task<bool> DeactivateTenantAsync(Guid tenantId);
    Task<Tenant?> GetTenantBySlugAsync(string slug);
}
