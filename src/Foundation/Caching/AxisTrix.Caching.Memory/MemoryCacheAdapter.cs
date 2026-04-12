using AxisResult;
using AxisTrix.Accessor;
using Microsoft.Extensions.Caching.Memory;

namespace AxisTrix.Caching.Memory;

public class MemoryCacheAdapter(IMemoryCache memoryCache, IAxisMediatorAccessor mediatorAccessor) : IAxisCache
{
    private readonly CancellationToken _cancellationToken = mediatorAccessor.AxisMediator!.CancellationToken;
    public Task<AxisResult<T?>> GetAsync<T>(string key)
    {
        return AxisResult.AxisResult.TryAsync(() =>
        {
            _cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(memoryCache.Get<T>(key));
        });
    }

    public Task<AxisResult.AxisResult> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        return AxisResult.AxisResult.TryAsync(() =>
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (expiration.HasValue)
                memoryCache.Set(key, value, expiration.Value);
            else
                memoryCache.Set(key, value);

            return Task.CompletedTask;
        });
    }

    public async Task<AxisResult<T>> GetOrCreateAsync<T>(string key, Func<Task<AxisResult<T>>> factory, TimeSpan? expiration = null)
    {
        try
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (memoryCache.TryGetValue(key, out T? value))
                return AxisResult.AxisResult.Ok(value!);

            var result = await factory();
            if (result.IsFailure)
                return result;

            if (expiration.HasValue)
                memoryCache.Set(key, result.Value, expiration.Value);
            else
                memoryCache.Set(key, result.Value);

            return result;
        }
        catch (Exception ex)
        {
            return AxisResult.AxisResult.Error<T>(AxisError.InternalServerError(ex.Message));
        }
    }

    public Task<AxisResult.AxisResult> RemoveAsync(string key)
    {
        return AxisResult.AxisResult.TryAsync(() =>
        {
            _cancellationToken.ThrowIfCancellationRequested();
            memoryCache.Remove(key);
            return Task.CompletedTask;
        });
    }

    public Task<AxisResult<bool>> ExistsAsync(string key)
    {
        return AxisResult.AxisResult.TryAsync(() =>
        {
            _cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(memoryCache.TryGetValue(key, out _));
        });
    }

}
