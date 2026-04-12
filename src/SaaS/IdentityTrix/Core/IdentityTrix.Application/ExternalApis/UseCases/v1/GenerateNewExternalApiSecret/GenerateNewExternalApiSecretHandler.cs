using AxisTrix.CQRS.Commands;
using AxisTrix.Results;
using IdentityTrix.Application.Authentication.Services;
using IdentityTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using IdentityTrix.Ports;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.GenerateNewExternalApiSecret;

internal class GenerateNewExternalApiSecretHandler(
    IUnitOfWorkProvider uowProvider,
    IExternalApiAggregateApplicationFactory factory,
    ICachedExternalApiSecretResolver cacheResolver
) : IAxisCommandHandler<GenerateNewExternalApiSecretCommand, GenerateNewExternalApiSecretResponse>
{
    public Task<AxisResult<GenerateNewExternalApiSecretResponse>> HandleAsync(GenerateNewExternalApiSecretCommand cmd)
    {
        ExternalApiId externalApiId = cmd.ExternalApiId;
        var plainSecret = ExternalApiSecret.Generate();
        var hashedSecret = ExternalApiSecret.Hash(plainSecret);

        return factory.GetByIdAsync(externalApiId)
            .ThenAsync(app => app.UpdateSecretAsync(hashedSecret))
            .ThenAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync())
            .ThenAsync(_ => cacheResolver.RemoveAsync(externalApiId))
            .MapAsync<IExternalApiAggregateApplication, GenerateNewExternalApiSecretResponse>(_ =>
                new GenerateNewExternalApiSecretResponse
                {
                    ExternalApiId = externalApiId,
                    Secret = plainSecret
                });
    }
}
