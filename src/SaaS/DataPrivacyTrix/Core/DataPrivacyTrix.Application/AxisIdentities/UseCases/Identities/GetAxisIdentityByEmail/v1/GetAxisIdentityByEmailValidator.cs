using AxisValidator;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByEmail;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Identities.GetAxisIdentityByEmail.v1;

internal class GetAxisIdentityByEmailValidator : AxisValidatorBase<GetAxisIdentityByEmailQuery>
{
    public GetAxisIdentityByEmailValidator()
    {
        RequiredEmail(x => x.EmailAddress, "EMAIL_ADDRESS_INVALID");
    }
}
