using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IOrganisationService
{
    Task<Khet360.Domain.Entities.OrganisationConfig?> GetConfigAsync();
    Task UpdateConfigAsync(Khet360.Domain.Entities.OrganisationConfig config);
}
