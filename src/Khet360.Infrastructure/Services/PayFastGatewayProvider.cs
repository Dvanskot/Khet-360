using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Infrastructure.Services;

public class PayFastGatewayProvider : IPaymentGatewayProvider
{
    public string ProviderName => "PayFast";

    public async Task<string> CreatePaymentLinkAsync(PaymentConfiguration config, decimal amount, string reference)
    {
        // In production, this would construct a PayFast POST request or redirect URL
        // using config.MerchantId and config.ApiKey (as the Passphrase)
        return $"https://www.payfast.co.za/onlinepayments/process?merchant_id={config.MerchantId}&amount={amount}&ref={reference}";
    }

    public async Task<bool> VerifyPaymentAsync(PaymentConfiguration config, string transactionRef, decimal amount)
    {
        // Mock verification logic using PayFast ITN (Instant Payment Notification)
        return await Task.FromResult(true);
    }
}
