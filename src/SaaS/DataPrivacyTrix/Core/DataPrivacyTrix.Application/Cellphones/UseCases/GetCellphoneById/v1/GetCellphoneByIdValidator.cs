using AxisValidator.FluentValidation;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneById;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetCellphoneById.v1;

internal class GetCellphoneByIdValidator : AxisValidatorBase<GetCellphoneByIdQuery>
{
    public GetCellphoneByIdValidator()
    {
        RequiredTryParse(x => x.CellphoneId, "CELLPHONE_ID_NULL_OR_NOT_VALID", CellphoneId.TryParse);
    }
}
