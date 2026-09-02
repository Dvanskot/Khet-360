using MediatR;
using Khet360.Domain.Entities;

namespace Khet360.Application.Tenants.Commands;

public record CreateTenantCommand(
    string Name,
    string Slug,
    Guid SubscriptionPlanId,
    IsolationTier Tier
) : IRequest<Guid>;
