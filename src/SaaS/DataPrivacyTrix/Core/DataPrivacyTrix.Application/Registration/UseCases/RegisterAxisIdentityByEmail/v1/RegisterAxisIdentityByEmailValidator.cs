using AxisValidator;
using DataPrivacyTrix.Application.Registration.UseCases.SharedData;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;
using FluentValidation;

namespace DataPrivacyTrix.Application.Registration.UseCases.RegisterAxisIdentityByEmail.v1;

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
