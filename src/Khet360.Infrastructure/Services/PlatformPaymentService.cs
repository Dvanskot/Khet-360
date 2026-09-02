using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class PlatformPaymentService : IPlatformPaymentService
{
    private readonly PlatformDbContext _platformDb;
    private readonly IEnumerable<IPaymentGatewayProvider> _gatewayProviders;
    private readonly ILogger<PlatformPaymentService> _logger;

    public PlatformPaymentService(PlatformDbContext platformDb, IEnumerable<IPaymentGatewayProvider> gatewayProviders, ILogger<PlatformPaymentService> logger)
    {
        _platformDb = platformDb;
        _gatewayProviders = gatewayProviders;
        _logger = logger;
    }

    public async Task<string> CreateSubscriptionPaymentLinkAsync(Guid planId, string customerEmail, string customerName)
    {
        var plan = await _platformDb.SubscriptionPlans.FindAsync(planId);
        if (plan == null) throw new KeyNotFoundException("Subscription plan not found.");

        var config = await _platformDb.PlatformPaymentConfigs.FirstOrDefaultAsync(c => c.IsActive);
        if (config == null)
        {
            throw new InvalidOperationException("Platform payment gateway is not configured.");
        }

        var provider = _gatewayProviders.FirstOrDefault(p =>
            p.ProviderName.Equals(config.ProviderName, StringComparison.OrdinalIgnoreCase));

        if (provider == null)
        {
            throw new NotSupportedException($"Payment provider {config.ProviderName} is not supported.");
        }

        string reference = $"SUB-NEW-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var paymentConfig = new PaymentConfiguration
        {
            Provider = Enum.Parse<PaymentProvider>(config.ProviderName, true),
            ApiKey = config.ApiKey,
            SecretKey = config.SecretKey,
            MerchantId = "PLATFORM_MERCHANT"
        };

        return await provider.CreatePaymentLinkAsync(paymentConfig, plan.MonthlyPrice, reference);
    }

    public async Task<bool> VerifySubscriptionPaymentAsync(string transactionRef, decimal amount)
    {
        var config = await _platformDb.PlatformPaymentConfigs.FirstOrDefaultAsync(c => c.IsActive);
        if (config == null) return false;

        var provider = _gatewayProviders.FirstOrDefault(p =>
            p.ProviderName.Equals(config.ProviderName, StringComparison.OrdinalIgnoreCase));

        if (provider == null) return false;

        var paymentConfig = new PaymentConfiguration
        {
            Provider = Enum.Parse<PaymentProvider>(config.ProviderName, true),
            ApiKey = config.ApiKey,
            SecretKey = config.SecretKey,
            MerchantId = "PLATFORM_MERCHANT"
        };

        return await provider.VerifyPaymentAsync(paymentConfig, transactionRef, amount);
    }
}
