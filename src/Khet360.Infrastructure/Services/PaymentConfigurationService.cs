using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class PaymentConfigurationService : IPaymentConfigurationService
{
    private readonly TenantDbContext _db;
    private readonly IEnumerable<IPaymentGatewayProvider> _providers;

    public PaymentConfigurationService(TenantDbContext db, IEnumerable<IPaymentGatewayProvider> providers)
    {
        _db = db;
        _providers = providers;
    }

    public async Task<PaymentConfiguration> GetConfigurationAsync()
    {
        return await _db.PaymentConfigurations.FirstOrDefaultAsync()
            ?? new PaymentConfiguration();
    }

    public async Task UpdateConfigurationAsync(PaymentConfiguration config)
    {
        var existing = await _db.PaymentConfigurations.FirstOrDefaultAsync();
        if (existing == null)
        {
            config.Id = Guid.NewGuid();
            _db.PaymentConfigurations.Add(config);
        }
        else
        {
            _db.Entry(existing).CurrentValues.SetValues(config);
        }

        config.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> TestConnectionAsync(PaymentConfiguration config)
    {
        var provider = _providers.FirstOrDefault(p =>
            p.ProviderName.Equals(config.Provider.ToString(), StringComparison.OrdinalIgnoreCase));

        if (provider == null) return false;

        try
        {
            // In a real app, this would call a specific "Ping" or "Auth" endpoint of the gateway
            // For this implementation, we assume a simple verification call
            return await provider.VerifyPaymentAsync(config, "test_connection", 0);
        }
        catch
        {
            return false;
        }
    }
}
