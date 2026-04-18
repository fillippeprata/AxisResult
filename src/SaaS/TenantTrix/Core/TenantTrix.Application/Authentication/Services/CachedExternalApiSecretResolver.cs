using Axis;
using TenantTrix.Application.ExternalApis;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.Authentication.Services;

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
