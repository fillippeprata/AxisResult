using AxisTrix.CQRS.Queries;
using AxisTrix.Results;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;
using DataPrivacyTrix.Ports.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetByCellphoneNumber.v1;

internal class GetByCellphoneNumberHandler(
    ICellphonesReaderPort readerPort
) : IAxisQueryHandler<GetByCellphoneNumberQuery, GetByCellphoneNumberResponse>
{
    public Task<AxisResult<GetByCellphoneNumberResponse>> HandleAsync(GetByCellphoneNumberQuery query) =>
        readerPort.GetByCellphoneNumberAsync(query.CountryId, query.CellphoneNumber!)
            .MapAsync(entity => new GetByCellphoneNumberResponse { CellphoneId = entity.CellphoneId});
}
