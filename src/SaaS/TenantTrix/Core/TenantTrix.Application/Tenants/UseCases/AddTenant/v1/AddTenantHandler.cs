using Axis;
using AxisMediator.Contracts.CQRS.Commands;
using TenantTrix.Contracts.Tenants.v1.AddTenant;
using TenantTrix.Ports;

namespace TenantTrix.Application.Tenants.UseCases.AddTenant.v1;

internal class AddTenantHandler(
    IUnitOfWorkProvider uowProvider,
    ITenantAggregateApplicationFactory factory
) : IAxisCommandHandler<AddTenantCommand, AddTenantResponse>
{
    public Task<AxisResult<AddTenantResponse>> HandleAsync(AddTenantCommand cmd)
        => factory.CreateAsync(new() { TenantName = cmd.TenantName! })
            .ThenAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync())
            .MapAsync<ITenantAggregateApplication, AddTenantResponse>(
                app => new AddTenantResponse
                {
                    TenantId = app.TenantId,
                    TenantName = app.TenantName
                });
}
