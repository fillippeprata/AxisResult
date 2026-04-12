using AxisTrix.Results;
using AxisTrix.Types.Localization;
using CountryId = AxisTrix.Types.Localization.CountryId;

namespace DataPrivacyTrix.Domain.Cellphones.Validation;

public static class LocalizationExtensions
{
    public static AxisResult<string> GetFormattedPhone(this CountryId countryId, string? phone)
    {
        if (countryId == CountryIds.Br)
        {
            var formatted = BrazilCellphoneValidator.Format(phone);
            if (formatted == null)
                return AxisError.ValidationRule("CELLPHONE_NUMBER_NULL_OR_NOT_VALID");
            return AxisResult.Ok(formatted);
        }

        return AxisError.Mapping("THERE_IS_NO_VALIDATION_FOR_THIS_COUNTRY");
    }

}
