using Axis;
using AxisMediator.Contracts;
using DataPrivacyTrix.Contracts.Cellphones.v1;
using DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneById;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneByNumber;

namespace DataPrivacyTrix.Sdk.Application.Cellphones.v1;

internal class CellphonesMediator(IAxisMediator mediator) : ICellphonesMediator
{
    public Task<AxisResult<AddCellphoneResponse>> AddAsync(AddCellphoneCommand command)
        => mediator.Cqrs.ExecuteAsync<AddCellphoneCommand, AddCellphoneResponse>(command);

    public Task<AxisResult<GetCellphoneByIdResponse>> GetByIdAsync(GetCellphoneByIdQuery query)
        => mediator.Cqrs.QueryAsync<GetCellphoneByIdQuery, GetCellphoneByIdResponse>(query);

    public Task<AxisResult<GetCellphoneByNumberResponse>> GetByCellphoneNumberAsync(GetCellphoneByNumberQuery query)
        => mediator.Cqrs.QueryAsync<GetCellphoneByNumberQuery, GetCellphoneByNumberResponse>(query);
}
