using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;
using IdentityTrix.Ports;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis.UseCases.AddExternalApi.v1;

internal class AddExternalApiHandler(
    IUnitOfWorkProvider uowProvider,
    IExternalApiAggregateApplicationFactory factory
) : IAxisCommandHandler<AddExternalApiCommand, AddExternalApiResponse>
{
    public Task<AxisResult<AddExternalApiResponse>> HandleAsync(AddExternalApiCommand cmd)
    {
        var plainSecret = ExternalApiSecret.Generate();
        var hashedSecret = ExternalApiSecret.Hash(plainSecret);

        return factory.CreateAsync(new() { ApiName = cmd.ApiName!, HashedSecret = hashedSecret, TenantId = cmd.TenantId! })
            .ThenAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync<IExternalApiAggregateApplication, AddExternalApiResponse>(
                app => new AddExternalApiResponse
                {
                    ExternalApiId = app.ExternalApiId,
                    Name = app.ApiName,
                    Secret = plainSecret,
                    TenantId = app.TenantId
                });
    }
}
