using AxisValidator;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityById;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.GetAxisIdentityById.v1;

internal class GetAxisIdentityByIdValidator : AxisValidatorBase<GetAxisIdentityByIdQuery>
{
    public GetAxisIdentityByIdValidator()
    {
        RequiredGuid7(x => x.AxisIdentityId, "AXIS_IDENTITY_ID_INVALID");
    }
}
