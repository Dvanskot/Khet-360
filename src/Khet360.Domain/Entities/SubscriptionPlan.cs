namespace Khet360.Domain.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public string Currency { get; set; } = "ZAR";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Entitlement> Entitlements { get; set; } = new List<Entitlement>();
    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
