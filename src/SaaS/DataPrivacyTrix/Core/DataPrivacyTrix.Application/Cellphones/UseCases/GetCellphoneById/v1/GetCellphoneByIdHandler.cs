using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneById;
using DataPrivacyTrix.Ports.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetCellphoneById.v1;

internal class GetCellphoneByIdHandler(
    ICellphonesReaderPort readerPort
) : IAxisQueryHandler<GetCellphoneByIdQuery, GetCellphoneByIdResponse>
{
    public Task<AxisResult<GetCellphoneByIdResponse>> HandleAsync(GetCellphoneByIdQuery query)
        => readerPort.GetByIdAsync(query.CellphoneId)
            .MapAsync(entity => new GetCellphoneByIdResponse { CountryId = entity.CountryId, CellphoneNumber = entity.CellphoneNumber });
}
