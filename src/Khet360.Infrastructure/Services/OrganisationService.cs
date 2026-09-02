using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Khet360.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class OrganisationService : IOrganisationService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantService _tenantService;

    public OrganisationService(TenantDbContext tenantDb, ITenantService tenantService)
    {
        _tenantDb = tenantDb;
        _tenantService = tenantService;
    }

    public async Task<OrganisationConfig?> GetConfigAsync()
    {
        return await _tenantDb.OrganisationConfigs.FirstOrDefaultAsync();
    }

    public async Task UpdateConfigAsync(OrganisationConfig config)
    {
        config.UpdatedAt = DateTime.UtcNow;

        var existing = await _tenantDb.OrganisationConfigs.FirstOrDefaultAsync();

        if (existing == null)
        {
            _tenantDb.OrganisationConfigs.Add(config);
        }
        else
        {
            _tenantDb.Entry(existing).CurrentValues.SetValues(config);
        }

        await _tenantDb.SaveChangesAsync();
    }
}
