using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface ITenantProvisioningService
{
    Task<string> ProvisionTenantAsync(Guid tenantId, string slug, IsolationTier tier);
}
