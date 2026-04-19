using Axis;
using DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneById;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneByNumber;

namespace DataPrivacyTrix.Contracts.Cellphones.v1;

public interface ICellphonesMediator
{
    Task<AxisResult<AddCellphoneResponse>> AddAsync(AddCellphoneCommand command);
    Task<AxisResult<GetCellphoneByIdResponse>> GetByIdAsync(GetCellphoneByIdQuery query);
    Task<AxisResult<GetCellphoneByNumberResponse>> GetByCellphoneNumberAsync(GetCellphoneByNumberQuery query);
}
