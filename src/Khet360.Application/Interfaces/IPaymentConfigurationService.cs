using System;
using System.Threading.Tasks;
using Khet360.Domain.Entities;

namespace Khet360.Application.Interfaces;

public interface IPaymentConfigurationService
{
    Task<PaymentConfiguration> GetConfigurationAsync();
    Task UpdateConfigurationAsync(PaymentConfiguration config);
    Task<bool> TestConnectionAsync(PaymentConfiguration config);
}
