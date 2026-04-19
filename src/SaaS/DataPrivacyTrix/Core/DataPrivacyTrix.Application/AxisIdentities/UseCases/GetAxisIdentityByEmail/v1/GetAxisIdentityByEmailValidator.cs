using AxisValidator;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByEmail;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.GetAxisIdentityByEmail.v1;

internal class GetAxisIdentityByEmailValidator : AxisValidatorBase<GetAxisIdentityByEmailQuery>
{
    public GetAxisIdentityByEmailValidator()
    {
        RequiredEmail(x => x.EmailAddress, "EMAIL_ADDRESS_INVALID");
    }
}
