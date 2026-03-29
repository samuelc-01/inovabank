using InovaBank.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace InovaBank.Infrastructure.Services.Cache;

public sealed class RedisCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var data = await cache.GetStringAsync(key, ct);
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(24)
        };

        var data = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, data, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) =>
        await cache.RemoveAsync(key, ct);
}
