using Axis;
using TenantTrix.Domain.Tenants.Root;
using TenantTrix.Ports.Tenants;
using TenantTrix.SharedKernel.Tenants;

namespace TenantTrix.Application.Tenants;

internal interface ITenantAggregateApplicationFactory
{
    Task<AxisResult<ITenantAggregateApplication>> GetByIdAsync(TenantId id);
    Task<AxisResult<ITenantAggregateApplication>> CreateAsync(NewArgs args);

    public record NewArgs
    {
        public required string TenantName { get; init; }
    }
}

internal class TenantAggregateApplicationFactory(
    ITenantsReaderPort readerPort,
    ITenantsWritePort writePort
) : ITenantAggregateApplicationFactory
{
    private ITenantAggregateApplication NewInstance(ITenantEntityProperties properties)
        => new TenantAggregateApplication(properties, readerPort, writePort);

    public Task<AxisResult<ITenantAggregateApplication>> GetByIdAsync(TenantId id)
        => readerPort.GetByIdAsync(id)
            .MapAsync(NewInstance);

    public Task<AxisResult<ITenantAggregateApplication>> CreateAsync(ITenantAggregateApplicationFactory.NewArgs args)
        => readerPort.GetByNameAsync(args.TenantName)
            .RequireNotFoundAsync(AxisError.ValidationRule("TENANT_NAME_ALREADY_EXISTS"))
            .WithValueAsync(new TenantEntity(TenantId.New, args.TenantName))
            .MapAsync(NewInstance)
            .ThenAsync(writePort.CreateAsync);
}
