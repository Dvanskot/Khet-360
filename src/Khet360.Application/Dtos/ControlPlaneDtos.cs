namespace Khet360.Application.Dtos;

public record RestoreRequest(Guid TenantId, Guid BackupJobId);

public record MigrationRequest(Guid TenantId, string TargetEnvironment);

public record TrialSignupDto(string CompanyName, string Slug, Guid SubscriptionPlanId, string Email);

public record SubscribeDto(string CompanyName, string Slug, Guid SubscriptionPlanId, string Email);

public record PaymentWebhookDto(string TransactionReference, decimal Amount, string CompanyName, string Slug, Guid SubscriptionPlanId);
