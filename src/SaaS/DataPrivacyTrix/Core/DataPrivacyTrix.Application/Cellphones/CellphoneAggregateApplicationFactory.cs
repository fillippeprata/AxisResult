using AxisTrix.Results;
using AxisTrix.Types;
using AxisTrix.Validation.Localization;
using DataPrivacyTrix.Domain.Cellphones.Root;
using DataPrivacyTrix.Ports.Cellphones;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones;

internal interface ICellphoneAggregateApplicationFactory
{
    Task<AxisResult<ICellphoneAggregateApplication>> GetByIdAsync(CellphoneId id);
    Task<AxisResult<ICellphoneAggregateApplication>> CreateAsync(NewArgs args);

    public record NewArgs
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
            .MapAsync(NewInstance);

    public Task<AxisResult<ICellphoneAggregateApplication>> GetByCellphoneNumberAsync(CountryId countryId, string cellphoneNumber)
        =>  countryId.GetFormattedPhone(cellphoneNumber)
            .ThenAsync(formattedNumber => readerPort.GetByCellphoneNumberAsync(countryId, formattedNumber))
            .MapAsync(NewInstance);

    public Task<AxisResult<ICellphoneAggregateApplication>> CreateAsync(ICellphoneAggregateApplicationFactory.NewArgs args)
        => GetByCellphoneNumberAsync(args.CountryId, args.CellphoneNumber)
            .RequireNotFoundAsync(AxisError.ValidationRule("CELLPHONE_ALREADY_EXISTS"))
            .WithValueAsync(new CellphoneEntity(CellphoneId.New, args.CountryId, args.CellphoneNumber))
            .ThenAsync(writePort.CreateAsync)
            .MapAsync(NewInstance);
}








