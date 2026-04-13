using AxisResult;
using AxisTrix.CQRS.Commands;
using IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;
using IdentityTrix.Ports;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.AddExternalApi;

internal class AddExternalApiHandler(
    IUnitOfWorkProvider uowProvider,
    IExternalApiAggregateApplicationFactory factory
) : IAxisCommandHandler<AddExternalApiCommand, AddExternalApiResponse>
{
    public Task<AxisResult<AddExternalApiResponse>> HandleAsync(AddExternalApiCommand cmd)
    {
        var plainSecret = ExternalApiSecret.Generate();
        var hashedSecret = ExternalApiSecret.Hash(plainSecret);

        return factory.CreateAsync(new() { ApiName = cmd.ApiName!, HashedSecret = hashedSecret })
            //todo: padrão para conferir se api name já existe para o tenant em questão
            .ThenAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync<IExternalApiAggregateApplication, AddExternalApiResponse>(
                app => new AddExternalApiResponse
                {
                    ExternalApiId = app.ExternalApiId,
                    Name = app.ApiName,
                    Secret = plainSecret
                });
    }
}
