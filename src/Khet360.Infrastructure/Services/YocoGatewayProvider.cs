using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Infrastructure.Services;

public class YocoGatewayProvider : IPaymentGatewayProvider
{
    public string ProviderName => "Yoco";

    public async Task<string> CreatePaymentLinkAsync(PaymentConfiguration config, decimal amount, string reference)
    {
        // In production, this would use Yoco's Payment Links API
        return $"https://pay.yoco.com/pay/{Guid.NewGuid():N}?amount={amount}&ref={reference}&merchant={config.MerchantId}";
    }

    public async Task<bool> VerifyPaymentAsync(PaymentConfiguration config, string transactionRef, decimal amount)
    {
        // Mock verification logic for Yoco
        return await Task.FromResult(true);
    }
}
