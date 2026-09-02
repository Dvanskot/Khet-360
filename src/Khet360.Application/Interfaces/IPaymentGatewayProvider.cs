using System.Threading.Tasks;
using Khet360.Domain.Entities;

namespace Khet360.Application.Interfaces;

public interface IPaymentGatewayProvider
{
    string ProviderName { get; }
    Task<string> CreatePaymentLinkAsync(PaymentConfiguration config, decimal amount, string reference);
    Task<bool> VerifyPaymentAsync(PaymentConfiguration config, string transactionRef, decimal amount);
}
