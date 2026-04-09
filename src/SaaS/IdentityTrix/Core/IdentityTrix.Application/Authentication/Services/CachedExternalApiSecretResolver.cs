using AxisTrix.Caching;
using AxisTrix.Results;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.Authentication.Services;

internal interface ICachedExternalApiSecretResolver
{
    Task<AxisResult> VerifySecretAsync(ExternalApiId externalApiId, string plainSecret);
    Task RemoveAsync(ExternalApiId externalApiId);
}

internal class CachedExternalApiSecretResolver(
    IAxisCache cache,
    IExternalApiReaderPort readerPort
) : ICachedExternalApiSecretResolver
{
    private static string CacheKey(ExternalApiId externalApiId) => $"external_api_secret_{externalApiId}";

    public Task<AxisResult> VerifySecretAsync(ExternalApiId externalApiId, string plainSecret)
    {
        var cacheKey = CacheKey(externalApiId);
        return cache.GetOrCreateAsync(cacheKey, () => readerPort.GetExternalApiByIdAsync(externalApiId))
            .ThenAsync(api => ExternalApiSecret.Verify(plainSecret, api.HashedSecret)
                    ? AxisResult.Ok()
                    : AxisError.Unauthorized("INVALID_EXTERNAL_API_ID_OR_SECRET"));
    }

    public Task RemoveAsync(ExternalApiId externalApiId) => cache.RemoveAsync(CacheKey(externalApiId));
}
