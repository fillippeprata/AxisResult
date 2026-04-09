using AxisTrix.CQRS.Commands;
using AxisTrix.Results;
using IdentityTrix.Application.Authentication.Services;
using IndentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace IdentityTrix.Application.Authentication.Handlers.v1;

internal class AuthenticateExternalApiHandler(
    ICachedExternalApiSecretResolver cachedSecretResolver
) : IAxisCommandHandler<AuthenticateExternalApiCommand>
{
    public async Task<AxisResult> HandleAsync(AuthenticateExternalApiCommand cmd)
        => await cachedSecretResolver.VerifySecretAsync(cmd.ExternalApiId, cmd.Secret);
}
