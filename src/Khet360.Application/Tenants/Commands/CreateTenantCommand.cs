using MediatR;

namespace Khet360.Application.Tenants.Commands;

public record CreateTenantCommand(
    string Name,
    string Slug,
    Guid SubscriptionPlanId,
    string ConnectionString
) : IRequest<Guid>;
