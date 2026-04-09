using AxisTrix.CQRS.Commands;
using AxisTrix.Results;
using IdentityTrix.Ports;
using IdentityTrix.SharedKernel.ExternalApis;
using IndentityTrix.Contracts.ExternalApis.v1.AddExternalApi;

namespace IdentityTrix.Application.ExternalApis.Handlers.v1;

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
            .TapAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync<IExternalApiAggregateApplication, AddExternalApiResponse>(app
                => new AddExternalApiResponse
                {
                    ExternalApiId = app.ExternalApiId,
                    Name = app.ApiName,
                    Secret = plainSecret
                });
    }
}
