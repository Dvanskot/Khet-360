using System.Threading.Tasks;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Interfaces;

public interface IPaymentGatewayProvider
{
    string ProviderName { get; }
    Task<string> CreatePaymentLinkAsync(PaymentConfiguration config, decimal amount, string reference);
    Task<bool> VerifyPaymentAsync(PaymentConfiguration config, string transactionRef, decimal amount);
}
