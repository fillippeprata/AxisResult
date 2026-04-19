using AxisValidator;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityById;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Identities.GetAxisIdentityById.v1;

internal class GetAxisIdentityByIdValidator : AxisValidatorBase<GetAxisIdentityByIdQuery>
{
    public GetAxisIdentityByIdValidator()
    {
        RequiredGuid7(x => x.AxisIdentityId, "AXIS_IDENTITY_ID_INVALID");
    }
}
