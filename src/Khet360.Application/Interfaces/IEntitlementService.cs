using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IEntitlementService
{
    Task<bool> IsEntitledAsync(Guid tenantId, string entitlementCode);
    Task<decimal> GetLimitAsync(Guid tenantId, string entitlementCode);
}
