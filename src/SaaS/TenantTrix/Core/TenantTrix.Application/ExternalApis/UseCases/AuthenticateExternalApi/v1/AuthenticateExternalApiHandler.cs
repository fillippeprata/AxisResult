using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using TenantTrix.Application.ExternalApis.Services;
using TenantTrix.Contracts.ExternalApis.v1.AuthenticateExternalApi;

namespace TenantTrix.Application.ExternalApis.UseCases.AuthenticateExternalApi.v1;

internal class AuthenticateExternalApiHandler(
    ICachedExternalApiSecretResolver cachedSecretResolver
) : IAxisCommandHandler<AuthenticateExternalApiCommand>
{
    public async Task<AxisResult> HandleAsync(AuthenticateExternalApiCommand cmd)
        => await cachedSecretResolver.GetExternalApiAsync(cmd.ExternalApiId)
            .ThenAsync(externalApiApp => externalApiApp.VerifySecret(cmd.Secret));
}
