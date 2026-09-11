namespace Khet360.Domain.Entities.Platform;

public class PlatformFeature
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FeatureFlagType Type { get; set; }
    public bool IsEnabledGlobally { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TenantFeatureOverride> TenantOverrides { get; set; } = new List<TenantFeatureOverride>();
}

public class TenantFeatureOverride
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public Guid FeatureId { get; set; }
    public PlatformFeature Feature { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum FeatureFlagType
{
    Boolean,
    Limit,
    Percentage,
    TimeWindow
}
