using Axis;
using TenantTrix.Domain.ExternalApis.Root;
using TenantTrix.Ports.ExternalApis;
using TenantTrix.SharedKernel.ExternalApis;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.ExternalApis;

internal interface IExternalApiAggregateApplicationFactory
{
    Task<AxisResult<IExternalApiAggregateApplication>> GetByIdAsync(ExternalApiId id);
    Task<AxisResult<IExternalApiAggregateApplication>> CreateAsync(NewArgs args);

    public record NewArgs
    {
        public required string ApiName { get; init; }
        public required string HashedSecret { get; init; }
        public required TenantId TenantId { get; init; }
    }
}

internal class ExternalApiAggregateApplicationFactory(
    IExternalApisReaderPort readerPort,
    IExternalApisWritePort writePort
) : IExternalApiAggregateApplicationFactory
{
    private IExternalApiAggregateApplication NewInstance(IExternalApiEntityProperties properties)
        => new ExternalApiAggregateApplication(properties, writePort);

    public Task<AxisResult<IExternalApiAggregateApplication>> GetByIdAsync(ExternalApiId id)
        => readerPort.GetByIdAsync(id)
            .MapAsync(NewInstance);

    public Task<AxisResult<IExternalApiAggregateApplication>> CreateAsync(IExternalApiAggregateApplicationFactory.NewArgs args)
        => readerPort.GetByNameAsync(args.ApiName)
            .RequireNotFoundAsync(AxisError.ValidationRule("EXTERNAL_API_NAME_ALREADY_EXISTS"))
            .WithValueAsync(new ExternalApiEntity(ExternalApiId.New, args.HashedSecret, args.ApiName, args.TenantId))
            .MapAsync(NewInstance)
            .ThenAsync(writePort.CreateAsync);
}
