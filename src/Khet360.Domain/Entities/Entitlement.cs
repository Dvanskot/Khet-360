namespace Khet360.Domain.Entities;

public class Entitlement
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g., "MAX_CASES_100"
    public string Description { get; set; } = string.Empty;
    public decimal LimitValue { get; set; } // 0 or 1 for boolean, >1 for quantitative limits
    public bool IsActive { get; set; }

    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
}
