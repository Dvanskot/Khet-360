using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Khet360.Infrastructure.Persistence;

public class TenantDbContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantService _tenantService;

    public TenantDbContextFactory(IServiceProvider serviceProvider, ITenantService tenantService)
    {
        _serviceProvider = serviceProvider;
        _tenantService = tenantService;
    }

    public TenantDbContext CreateDbContext()
    {
        var tenant = _tenantService.CurrentTenant;
        if (tenant == null)
        {
            throw new InvalidOperationException("No tenant has been resolved for the current request.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseSqlServer(tenant.ConnectionString);

        // Resolve ITenantUserContext from the service provider
        var userContext = _serviceProvider.GetRequiredService<ITenantUserContext>();

        return new TenantDbContext(optionsBuilder.Options, userContext);
    }
}
