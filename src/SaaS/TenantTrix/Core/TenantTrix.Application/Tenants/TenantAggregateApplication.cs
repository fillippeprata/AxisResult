using Axis;
using TenantTrix.Domain.Tenants.Root;
using TenantTrix.Ports.Tenants;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.Application.Tenants;

internal interface ITenantAggregateApplication : ITenantEntityProperties
{
    Task<AxisResult> UpdateNameAsync(string tenantName);
}

internal class TenantAggregateApplication(
    ITenantEntityProperties properties,
    ITenantsReaderPort readerPort,
    ITenantsWritePort writePort
) : TenantEntity(properties), ITenantAggregateApplication
{
    public Task<AxisResult> UpdateNameAsync(string tenantName)
        => readerPort.GetByNameAsync(tenantName)
            .EnsureAsync(
                existing => existing.TenantId == TenantId,
                AxisError.ValidationRule("TENANT_NAME_ALREADY_EXISTS"))
            .RecoverNotFoundAsync(() => this)
            .ThenAsync(_ => writePort.UpdateNameAsync(TenantId, tenantName))
            .MatchAsync(onSuccess: _ => AxisResult.Ok(), AxisResult.Error);
}
