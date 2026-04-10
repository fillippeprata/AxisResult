using AxisTrix.CQRS.Commands;
using AxisTrix.Results;
using IdentityTrix.Application.Authentication.Services;
using IndentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace IdentityTrix.Application.Authentication.UseCases.v1.AuthenticateExternalApi;

internal class AuthenticateExternalApiHandler(
    ICachedExternalApiSecretResolver cachedSecretResolver
) : IAxisCommandHandler<AuthenticateExternalApiCommand>
{
    public async Task<AxisResult> HandleAsync(AuthenticateExternalApiCommand cmd)
        => await cachedSecretResolver.GetExternalApiAsync(cmd.ExternalApiId)
            .ThenAsync(externalApiApp => externalApiApp.VerifySecret(cmd.Secret));
}
