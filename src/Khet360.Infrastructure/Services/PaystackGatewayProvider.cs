using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Infrastructure.Services;

public class PaystackGatewayProvider : IPaymentGatewayProvider
{
    public string ProviderName => "Paystack";

    public async Task<string> CreatePaymentLinkAsync(PaymentConfiguration config, decimal amount, string reference)
    {
        // In production, this would call Paystack's Transaction/Initialize API
        return $"https://checkout.paystack.com/pay/{Guid.NewGuid():N}?amount={amount}&ref={reference}&merchant={config.MerchantId}";
    }

    public async Task<bool> VerifyPaymentAsync(PaymentConfiguration config, string transactionRef, decimal amount)
    {
        // Mock verification logic for Paystack
        return await Task.FromResult(true);
    }
}
