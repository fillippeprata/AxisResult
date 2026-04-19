using AxisValidator;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityById;

namespace DataPrivacyTrix.Application.Registration.UseCases.GetAxisIdentityById.v1;

internal class GetAxisIdentityByIdValidator : AxisValidatorBase<GetAxisIdentityByIdQuery>
{
    public GetAxisIdentityByIdValidator()
    {
        RequiredGuid7(x => x.AxisIdentityId, "AXIS_IDENTITY_ID_INVALID");
    }
}
