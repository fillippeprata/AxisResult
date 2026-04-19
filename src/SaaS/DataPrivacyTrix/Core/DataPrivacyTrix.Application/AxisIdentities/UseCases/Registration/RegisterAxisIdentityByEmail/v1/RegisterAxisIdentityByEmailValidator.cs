using AxisValidator;
using DataPrivacyTrix.Application.AxisIdentities.UseCases.Registration.SharedData;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByEmail;
using FluentValidation;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Registration.RegisterAxisIdentityByEmail.v1;

internal class RegisterAxisIdentityByEmailValidator : AxisValidatorBase<RegisterAxisIdentityByEmailCommand>
{
    public RegisterAxisIdentityByEmailValidator()
    {
        NotNullOrEmpty(x => x.Data, "DATA_REQUIRED");
        RuleFor(x => x.Data!)
            .SetValidator(new RegisterAxisIdentityDataValidator())
            .When(x => x.Data is not null);
        RequiredGuid7(x => x.EmailId, "EMAIL_ID_INVALID");
    }
}
