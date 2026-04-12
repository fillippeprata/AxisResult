using AxisTrix.CQRS.Queries;
using AxisTrix.Results;
using AxisTrix.Types;
using AxisTrix.Validation.Localization;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;
using DataPrivacyTrix.Ports.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetByCellphoneNumber.v1;

internal class GetByCellphoneNumberHandler(
    ICellphonesReaderPort readerPort
) : IAxisQueryHandler<GetByCellphoneNumberQuery, GetByCellphoneNumberResponse>
{
    public Task<AxisResult<GetByCellphoneNumberResponse>> HandleAsync(GetByCellphoneNumberQuery query)
    {
        CountryId countryId = query.CountryId;

        return countryId.GetFormattedPhone(query.CellphoneNumber)
            .ThenAsync(formattedPhone => readerPort.GetByCellphoneNumberAsync(countryId, formattedPhone))
            .MapAsync(entity => new GetByCellphoneNumberResponse { CellphoneId = entity.CellphoneId});
    }
}
