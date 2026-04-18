using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using TenantTrix.Contracts.ExternalApis.v1.EditExternalApi;
using TenantTrix.Ports;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.ExternalApis.UseCases.EditExternalApi.v1;

internal class EditExternalApiHandler(
    IUnitOfWorkProvider uowProvider,
    IExternalApiAggregateApplicationFactory factory
) : IAxisCommandHandler<EditExternalApiCommand, EditExternalApiResponse>
{
    public Task<AxisResult<EditExternalApiResponse>> HandleAsync(EditExternalApiCommand cmd)
    {
        ExternalApiId externalApiId = cmd.ExternalApiId!;
        var newName = cmd.ApiName!;

        return factory.GetByIdAsync(externalApiId)
            .ThenAsync(app => app.UpdateNameAsync(newName))
            .ThenAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync<IExternalApiAggregateApplication, EditExternalApiResponse>(_ =>
                new EditExternalApiResponse
                {
                    ExternalApiId = externalApiId,
                    ApiName = newName
                });
    }
}
