using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Infrastructure.Services;

public class PeachPaymentsGatewayProvider : IPaymentGatewayProvider
{
    public string ProviderName => "PeachPayments";

    public async Task<string> CreatePaymentLinkAsync(PaymentConfiguration config, decimal amount, string reference)
    {
        // In production, this would call Peach Payments API to create a payment session
        return $"https://pay.peachpayments.com/checkout/{Guid.NewGuid():N}?amount={amount}&ref={reference}&merchant={config.MerchantId}";
    }

    public async Task<bool> VerifyPaymentAsync(PaymentConfiguration config, string transactionRef, decimal amount)
    {
        // Mock verification logic for Peach Payments
        return await Task.FromResult(true);
    }
}
