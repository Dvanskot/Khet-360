using System;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IEntitlementService
{
    Task<bool> IsEntitledAsync(Guid tenantId, string entitlementCode);
    Task<decimal> GetLimitAsync(Guid tenantId, string entitlementCode);
}
