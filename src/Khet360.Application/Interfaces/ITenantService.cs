namespace Khet360.Application.Interfaces;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;


public interface ITenantService
{
    Tenant? CurrentTenant { get; }
    void SetTenant(Tenant tenant);
}
