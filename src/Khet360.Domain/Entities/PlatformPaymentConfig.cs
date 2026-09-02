namespace Khet360.Domain.Entities;

public class PlatformPaymentConfig
{
    public Guid Id { get; set; }
    public string ProviderName { get; set; } = string.Empty; // e.g., "Stripe", "PayFast"
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
