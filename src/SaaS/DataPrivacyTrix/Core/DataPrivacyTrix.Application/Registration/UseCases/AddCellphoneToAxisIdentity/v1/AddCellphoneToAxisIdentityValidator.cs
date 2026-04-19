using AxisValidator;
using DataPrivacyTrix.Contracts.Registration.v1.AddCellphoneToAxisIdentity;

namespace DataPrivacyTrix.Application.Registration.UseCases.AddCellphoneToAxisIdentity.v1;

internal class AddCellphoneToAxisIdentityValidator : AxisValidatorBase<AddCellphoneToAxisIdentityCommand>
{
    public AddCellphoneToAxisIdentityValidator()
    {
        RequiredGuid7(x => x.AxisIdentityId, "AXIS_IDENTITY_ID_INVALID");
        RequiredGuid7(x => x.CellphoneId, "CELLPHONE_ID_INVALID");
    }
}
