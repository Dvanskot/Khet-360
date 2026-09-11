using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Interfaces;

public interface IPlatformCacheService
{
    Task<List<Tenant>> GetTenantsAsync();
    Task<List<SubscriptionPlan>> GetSubscriptionPlansAsync();
    Task<List<Entitlement>> GetEntitlementsAsync();
    Task<List<TaxYear>> GetTaxYearsAsync();
    Task<List<TaxBracket>> GetTaxBracketsAsync();
    Task<List<StatutoryRate>> GetStatutoryRatesAsync();
    Task<List<LeaveType>> GetLeaveTypesAsync();
    Task<List<Position>> GetPositionsAsync();

    Task InvalidateTenantsAsync();
    Task InvalidateSubscriptionPlansAsync();
    Task InvalidateEntitlementsAsync();
    Task InvalidateAllAsync();
}
