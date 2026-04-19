using Axis;
using DataPrivacyTrix.Domain.Cellphones.Root;
using DataPrivacyTrix.Ports.Cellphones;
using DataPrivacyTrix.SharedKernel.Cellphones;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Application.Cellphones;

internal interface ICellphoneAggregateApplicationFactory
{
    Task<AxisResult<ICellphoneAggregateApplication>> GetByIdAsync(CellphoneId id);
    Task<AxisResult<ICellphoneAggregateApplication>> CreateAsync(NewArgs args);

    internal record NewArgs
    {
        public required CountryId CountryId { get; init; }
        public required string CellphoneNumber { get; init; }
    }
}

internal class CellphoneAggregateApplicationFactory(
    ICellphonesReaderPort readerPort,
    ICellphonesWritePort writePort
) : ICellphoneAggregateApplicationFactory
{
    private static ICellphoneAggregateApplication NewInstance(ICellphoneEntityProperties properties)
        => new CellphoneAggregateApplication(properties);

    public Task<AxisResult<ICellphoneAggregateApplication>> GetByIdAsync(CellphoneId id)
        => readerPort.GetByIdAsync(id)
            .MapAsync(NewInstance)
            .ActionAsync(app => app.IsValidAsync());

    public Task<AxisResult<ICellphoneAggregateApplication>> GetByCellphoneNumberAsync(CountryId countryId, string cellphoneNumber)
        =>  readerPort.GetByCellphoneNumberAsync(countryId, cellphoneNumber)
            .MapAsync(NewInstance);

    public Task<AxisResult<ICellphoneAggregateApplication>> CreateAsync(ICellphoneAggregateApplicationFactory.NewArgs args)
        => GetByCellphoneNumberAsync(args.CountryId, args.CellphoneNumber)
            .RequireNotFoundAsync(AxisError.ValidationRule("CELLPHONE_ALREADY_EXISTS"))
            .WithValueAsync(new CellphoneEntity(CellphoneId.New, args.CountryId, args.CellphoneNumber))
            .MapAsync(NewInstance)
            .ActionAsync(app => app.IsValidAsync())
            .ThenAsync(writePort.CreateAsync);
}








