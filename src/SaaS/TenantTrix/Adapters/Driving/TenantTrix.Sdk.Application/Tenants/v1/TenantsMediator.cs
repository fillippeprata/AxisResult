using Axis;
using AxisMediator.Contracts;
using TenantTrix.Contracts.Tenants.v1;
using TenantTrix.Contracts.Tenants.v1.AddTenant;
using TenantTrix.Contracts.Tenants.v1.EditTenant;

namespace TenantTrix.Sdk.Application.Tenants.v1;

internal class TenantsMediator(IAxisMediator mediator) : ITenantsMediator
{
    public Task<AxisResult<AddTenantResponse>> AddAsync(AddTenantCommand command)
        => mediator.Cqrs.ExecuteAsync<AddTenantCommand, AddTenantResponse>(command);

    public Task<AxisResult<EditTenantResponse>> EditAsync(EditTenantCommand command)
        => mediator.Cqrs.ExecuteAsync<EditTenantCommand, EditTenantResponse>(command);
}
