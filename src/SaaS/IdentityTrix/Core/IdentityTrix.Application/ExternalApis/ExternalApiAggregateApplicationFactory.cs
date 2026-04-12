using AxisTrix.Results;
using IdentityTrix.Domain.ExternalApis.Root;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis;

internal interface IExternalApiAggregateApplicationFactory
{
    Task<AxisResult<IExternalApiAggregateApplication>> GetByIdAsync(ExternalApiId id);
    Task<AxisResult<IExternalApiAggregateApplication>> CreateAsync(NewArgs args);

    public record NewArgs
    {
        public required string ApiName { get; init; }
        public required string HashedSecret { get; init; }
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

    //todo: Verificar se api name já existe. Usar mesmo padrão de DataPrivacyTrix.Cellphones
    public Task<AxisResult<IExternalApiAggregateApplication>> CreateAsync(IExternalApiAggregateApplicationFactory.NewArgs args)
        => AxisResult.Ok<IExternalApiEntityProperties>(new ExternalApiEntity(ExternalApiId.New, args.HashedSecret, args.ApiName))
            .TapAsync(writePort.CreateAsync)
            .MapAsync(NewInstance);
}
