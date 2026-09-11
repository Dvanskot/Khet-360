using FluentValidation;
using Khet360.Application.Dtos;

namespace Khet360.Application.Validators;

public class CreateTenantDtoValidator : AbstractValidator<CreateTenantDto>
{
    public CreateTenantDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$");
        RuleFor(x => x.SubscriptionPlanId).NotEmpty();
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
    }
}

public class UpdateTenantDtoValidator : AbstractValidator<UpdateTenantDto>
{
    public UpdateTenantDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}

public class SuspendTenantDtoValidator : AbstractValidator<SuspendTenantDto>
{
    public SuspendTenantDtoValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class CancelTenantDtoValidator : AbstractValidator<CancelTenantDto>
{
    public CancelTenantDtoValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class ReactivateTenantDtoValidator : AbstractValidator<ReactivateTenantDto>
{
    public ReactivateTenantDtoValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(256);
    }
}

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RestoreRequestValidator : AbstractValidator<RestoreRequest>
{
    public RestoreRequestValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.BackupJobId).NotEmpty();
    }
}

public class MigrationRequestValidator : AbstractValidator<MigrationRequest>
{
    public MigrationRequestValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.TargetEnvironment).NotEmpty().MaximumLength(100);
    }
}

public class TrialSignupDtoValidator : AbstractValidator<TrialSignupDto>
{
    public TrialSignupDtoValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$");
        RuleFor(x => x.SubscriptionPlanId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
    }
}

public class SubscribeDtoValidator : AbstractValidator<SubscribeDto>
{
    public SubscribeDtoValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$");
        RuleFor(x => x.SubscriptionPlanId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
    }
}

public class PaymentWebhookDtoValidator : AbstractValidator<PaymentWebhookDto>
{
    public PaymentWebhookDtoValidator()
    {
        RuleFor(x => x.TransactionReference).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$");
        RuleFor(x => x.SubscriptionPlanId).NotEmpty();
    }
}
