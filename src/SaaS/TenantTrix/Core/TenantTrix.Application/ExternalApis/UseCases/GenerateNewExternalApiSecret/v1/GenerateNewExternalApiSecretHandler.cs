using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using TenantTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using TenantTrix.Application.Authentication.Services;
using TenantTrix.Ports;
using TenantTrix.SharedKernel.ExternalApis;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.ExternalApis.UseCases.GenerateNewExternalApiSecret.v1;

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
