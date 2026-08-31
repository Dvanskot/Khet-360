using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;

namespace Khet360.Infrastructure.Services;

public class TenantService : ITenantService
{
    public Tenant? CurrentTenant { get; private set; }

    public void SetTenant(Tenant tenant)
    {
        CurrentTenant = tenant;
    }
}
