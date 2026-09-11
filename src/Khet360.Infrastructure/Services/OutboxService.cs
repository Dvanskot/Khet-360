using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Text.Json;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Infrastructure.Services;

public class OutboxService : IOutboxService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public OutboxService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task EnqueueAsync<T>(T eventMessage, CancellationToken cancellationToken = default) where T : class
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found for outbox message.");

        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = typeof(T).FullName ?? typeof(T).Name,
            Content = JsonSerializer.Serialize(eventMessage),
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.OutboxMessages.Add(message);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
