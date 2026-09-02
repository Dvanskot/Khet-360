using System;
using System.Threading.Tasks;
using Khet360.Domain.Entities;

namespace Khet360.Application.Interfaces;

public interface ITenantProvisioningService
{
    Task<string> ProvisionTenantAsync(Guid tenantId, string slug, IsolationTier tier);
}
