using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface ITenantProvisioningService
{
    Task ProvisionTenantAsync(Guid tenantId, string slug, string connectionString);
}
