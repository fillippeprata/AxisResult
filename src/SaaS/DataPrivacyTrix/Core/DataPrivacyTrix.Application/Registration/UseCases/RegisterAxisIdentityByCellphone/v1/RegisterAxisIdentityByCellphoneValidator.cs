using AxisValidator;
using DataPrivacyTrix.Application.Registration.UseCases.SharedData;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using FluentValidation;

namespace DataPrivacyTrix.Application.Registration.UseCases.RegisterAxisIdentityByCellphone.v1;

internal class RegisterAxisIdentityByCellphoneValidator : AxisValidatorBase<RegisterAxisIdentityByCellphoneCommand>
{
    public RegisterAxisIdentityByCellphoneValidator()
    {
        NotNullOrEmpty(x => x.Data, "DATA_REQUIRED");
        RuleFor(x => x.Data!)
            .SetValidator(new RegisterAxisIdentityDataValidator())
            .When(x => x.Data is not null);
        RequiredGuid7(x => x.CellphoneId, "CELLPHONE_ID_INVALID");
    }
}
