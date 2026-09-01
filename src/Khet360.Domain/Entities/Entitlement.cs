namespace Khet360.Domain.Entities;

public class Entitlement
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g., "MAX_CASES_100"
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}
