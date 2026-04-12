using AxisTrix;
using AxisTrix.Caching;
using IdentityTrix.Application.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.Authentication.Services;

internal interface ICachedExternalApiSecretResolver
{
    Task<AxisResult> RemoveAsync(ExternalApiId externalApiId);
    Task<AxisResult<IExternalApiAggregateApplication>> GetExternalApiAsync(ExternalApiId externalApiId);
}

internal class CachedExternalApiSecretResolver(
    IAxisCache cache,
    IExternalApiAggregateApplicationFactory factory
) : ICachedExternalApiSecretResolver
{
    private static string CacheKey(ExternalApiId externalApiId) => $"external_api_secret_{externalApiId}";

    public Task<AxisResult> RemoveAsync(ExternalApiId externalApiId)
        => cache.RemoveAsync(CacheKey(externalApiId));

    public Task<AxisResult<IExternalApiAggregateApplication>> GetExternalApiAsync(ExternalApiId externalApiId)
        => cache.GetOrCreateAsync(CacheKey(externalApiId), () => factory.GetByIdAsync(externalApiId));
}
