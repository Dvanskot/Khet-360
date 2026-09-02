using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Infrastructure.Services;

public class StripeGatewayProvider : IPaymentGatewayProvider
{
    public string ProviderName => "Stripe";

    public async Task<string> CreatePaymentLinkAsync(PaymentConfiguration config, decimal amount, string reference)
    {
        // In production, this would use config.ApiKey to call Stripe's Checkout Sessions API
        // Example: Stripe.Checkout.Session.Create(...)
        return $"https://checkout.stripe.com/pay/{Guid.NewGuid():N}?amount={amount}&ref={reference}&api_key={config.ApiKey}";
    }

    public async Task<bool> VerifyPaymentAsync(PaymentConfiguration config, string transactionRef, decimal amount)
    {
        // Mock verification logic using Stripe API
        // In production, you would verify the PaymentIntent status
        return await Task.FromResult(true);
    }
}
