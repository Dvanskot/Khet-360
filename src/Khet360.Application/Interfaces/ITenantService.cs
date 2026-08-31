namespace Khet360.Application.Interfaces;

using Khet360.Domain.Entities;

public interface ITenantService
{
    Tenant? CurrentTenant { get; }
    void SetTenant(Tenant tenant);
}
