using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IMigrationService
{
    Task<bool> MigrateTenantAsync(Guid tenantId, string targetEnvironment);
}
