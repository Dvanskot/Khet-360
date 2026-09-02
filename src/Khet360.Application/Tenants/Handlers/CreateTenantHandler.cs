using MediatR;
using Khet360.Application.Tenants.Commands;
using Khet360.Application.Interfaces;

namespace Khet360.Application.Tenants.Handlers;

public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantManagementService _tenantManagementService;

    public CreateTenantHandler(ITenantManagementService tenantManagementService)
    {
        _tenantManagementService = tenantManagementService;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantManagementService.CreateTenantAsync(
            request.Name,
            request.Slug,
            request.SubscriptionPlanId,
            request.Tier);

        return tenant.Id;
    }
}
