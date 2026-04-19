using AxisValidator;
using DataPrivacyTrix.Application.AxisIdentities.UseCases.Registration.SharedData;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByCellphone;
using FluentValidation;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Registration.RegisterAxisIdentityByCellphone.v1;

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
