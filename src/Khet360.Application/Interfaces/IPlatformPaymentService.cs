using System.Threading.Tasks;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Interfaces;

public interface IPlatformPaymentService
{
    Task<string> CreateSubscriptionPaymentLinkAsync(Guid planId, string customerEmail, string customerName);
    Task<bool> VerifySubscriptionPaymentAsync(string transactionRef, decimal amount, string payload, string signatureHeader);
}
