using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneByNumber;
using DataPrivacyTrix.Ports.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetCellphoneByNumber.v1;

internal class GetCellphoneByNumberHandler(
    ICellphonesReaderPort readerPort
) : IAxisQueryHandler<GetCellphoneByNumberQuery, GetCellphoneByNumberResponse>
{
    public Task<AxisResult<GetCellphoneByNumberResponse>> HandleAsync(GetCellphoneByNumberQuery query) =>
        readerPort.GetByCellphoneNumberAsync(query.CountryId, query.CellphoneNumber!)
            .MapAsync(entity => new GetCellphoneByNumberResponse { CellphoneId = entity.CellphoneId});
}
