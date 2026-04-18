using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using TenantTrix.Contracts.Tenants.v1.EditTenant;
using TenantTrix.Ports;

namespace TenantTrix.Application.Tenants.UseCases.EditTenant.v1;

internal class EditTenantHandler(
    IUnitOfWorkProvider uowProvider,
    ITenantAggregateApplicationFactory factory
) : IAxisCommandHandler<EditTenantCommand, EditTenantResponse>
{
    public Task<AxisResult<EditTenantResponse>> HandleAsync(EditTenantCommand cmd)
    {
        TenantId tenantId = cmd.TenantId!;
        var newName = cmd.TenantName!;

        return factory.GetByIdAsync(tenantId)
            .ThenAsync(app => app.UpdateNameAsync(newName))
            .ThenAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync<ITenantAggregateApplication, EditTenantResponse>(_ =>
                new EditTenantResponse
                {
                    TenantId = tenantId,
                    TenantName = newName
                });
    }
}
