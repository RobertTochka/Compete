using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using System.Text.Json;

namespace compete_platform;
public static class CacheExtensions
{
    public async static Task<T?> GetObjectAsync<T>(this IDistributedCache cache, string cacheKey)
    {
        try
        {
            var cacheObj = await cache.GetStringAsync(cacheKey);
            if (cacheObj == null)
                return default(T?);
            return JsonSerializer.Deserialize<T>(cacheObj);
        }
        catch (Exception)
        {
            Log.Logger.Error($"Redis cache is not available.Try to get object is failed\n");
            return default(T);
        }
    }
    public async static Task SetObjectAsync<T>(this IDistributedCache cache, string cacheKey, T instance,
        DistributedCacheEntryOptions? options = null)
    {
        try
        {
            if (options == null)
                options = new() { AbsoluteExpiration = DateTime.Now.AddHours(4) };
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(instance), options);
        }
        catch (Exception)
        {
            Log.Logger.Error($"Redis cache is not avaialble. Try to set object is failed.\n");
        }
    }
}
