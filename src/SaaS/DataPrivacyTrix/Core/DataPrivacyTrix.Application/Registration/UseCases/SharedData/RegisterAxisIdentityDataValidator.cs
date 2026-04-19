using Axis.Localization;
using AxisValidator;
using AxisValidator.Brazil;
using DataPrivacyTrix.Contracts.Registration.v1.SharedData;
using FluentValidation;
using DataPrivacySecurityLevel = DataPrivacyTrix.SharedKernel.Registration.SecurityLevel;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Application.Registration.UseCases.SharedData;

internal class RegisterAxisIdentityDataValidator : AxisValidatorBase<RegisterAxisIdentityData>
{
    public RegisterAxisIdentityDataValidator()
    {
        NotNullOrEmpty(x => x.IsIndividual, "IS_INDIVIDUAL_REQUIRED");
        RequiredWithMaxLength(x => x.Document, "DOCUMENT_REQUIRED", 14);
        RequiredWithMaxLength(x => x.DisplayName, "DISPLAY_NAME_REQUIRED");
        NotNullOrEmpty(x => x.DefaultLanguage, "DEFAULT_LANGUAGE_REQUIRED");
        RequiredTryParse(x => x.CountryId, "COUNTRY_ID_REQUIRED",
            value => value is not null && CountryId.TryParse(value.ToString(), out _));
        RequiredTryParse(x => x.SecurityLevel, "SECURITY_LEVEL_INVALID",
            value => value is not null && Enum.TryParse<DataPrivacySecurityLevel>(value.ToString(), ignoreCase: true, out _));

        RuleFor(x => x.Document)
            .Must((data, doc) =>
            {
                if (data.IsIndividual != true) return true;
                if (!CountryId.TryParse(data.CountryId, out var countryId) || countryId != CountryIds.Br) return true;
                return CpfValidator.Validate(doc);
            })
            .WithErrorCode("DOCUMENT_INVALID")
            .When(x => !string.IsNullOrWhiteSpace(x.Document));
    }
}
