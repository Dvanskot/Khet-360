using Khet360.Application.Interfaces;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Khet360.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;

    public CacheService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RedisConnection")
            ?? "localhost:6379";
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value!.ToString());
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry.HasValue ? (Expiration)expiry.Value : default);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task ClearAsync()
    {
        // Note: In a production environment, avoid FLUSHDB on shared instances.
        // We can use a prefix for Khet360 keys and delete by pattern if needed.
        await _db.ExecuteAsync("FLUSHDB");
    }
}
