using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IPlatformPaymentService
{
    Task<string> CreateSubscriptionPaymentLinkAsync(Guid planId, string customerEmail, string customerName);
    Task<bool> VerifySubscriptionPaymentAsync(string transactionRef, decimal amount);
}
