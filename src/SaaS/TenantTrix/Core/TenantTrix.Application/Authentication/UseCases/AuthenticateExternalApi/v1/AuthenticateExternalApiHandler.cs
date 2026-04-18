using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using TenantTrix.Contracts.Authentication.v1.AuthenticateExternalApi;
using TenantTrix.Application.Authentication.Services;

namespace TenantTrix.Application.Authentication.UseCases.AuthenticateExternalApi.v1;

internal class AuthenticateExternalApiHandler(
    ICachedExternalApiSecretResolver cachedSecretResolver
) : IAxisCommandHandler<AuthenticateExternalApiCommand>
{
    public async Task<AxisResult> HandleAsync(AuthenticateExternalApiCommand cmd)
        => await cachedSecretResolver.GetExternalApiAsync(cmd.ExternalApiId)
            .ThenAsync(externalApiApp => externalApiApp.VerifySecret(cmd.Secret));
}
