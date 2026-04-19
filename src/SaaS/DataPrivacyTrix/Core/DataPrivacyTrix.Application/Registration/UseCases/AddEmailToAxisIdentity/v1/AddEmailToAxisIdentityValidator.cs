using AxisValidator;
using DataPrivacyTrix.Contracts.Registration.v1.AddEmailToAxisIdentity;

namespace DataPrivacyTrix.Application.Registration.UseCases.AddEmailToAxisIdentity.v1;

internal class AddEmailToAxisIdentityValidator : AxisValidatorBase<AddEmailToAxisIdentityCommand>
{
    public AddEmailToAxisIdentityValidator()
    {
        RequiredGuid7(x => x.AxisIdentityId, "AXIS_IDENTITY_ID_INVALID");
        RequiredGuid7(x => x.EmailId, "EMAIL_ID_INVALID");
    }
}
