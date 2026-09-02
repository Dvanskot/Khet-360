using System;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Khet360.Infrastructure.Services;

public interface IWebhookValidator
{
    Task<bool> ValidateSignatureAsync(PaymentConfiguration config, string payload, string signatureHeader);
}

public class WebhookValidator : IWebhookValidator
{
    private readonly ILogger<WebhookValidator> _logger;

    public WebhookValidator(ILogger<WebhookValidator> logger)
    {
        _logger = logger;
    }

    public async Task<bool> ValidateSignatureAsync(PaymentConfiguration config, string payload, string signatureHeader)
    {
        if (string.IsNullOrEmpty(config.WebhookSecret))
        {
            _logger.LogWarning("Webhook secret not configured for tenant. Skipping signature verification.");
            return false;
        }

        try
        {
            // Standard HMAC-SHA256 verification used by most modern gateways (Stripe, Paystack, etc.)
            var secretBytes = Encoding.UTF8.GetBytes(config.WebhookSecret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(secretBytes);
            var hashBytes = hmac.ComputeHash(payloadBytes);
            var calculatedSignature = Convert.ToHexString(hashBytes).ToLower();

            return string.Equals(calculatedSignature, signatureHeader.ToLower(), StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while validating webhook signature.");
            return false;
        }
    }
}
