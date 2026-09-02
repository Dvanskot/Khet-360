namespace Khet360.Domain.Entities;

public enum IsolationTier
{
    Isolated, // Dedicated DB, shared server (current)
    Dedicated // Dedicated server
}

public enum SubscriptionStatus
{
    Trial,
    Active,
    PastDue,
    Suspended,
    Canceled
}

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // e.g., "tenanta"
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;
    public IsolationTier Tier { get; set; } = IsolationTier.Isolated;
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
