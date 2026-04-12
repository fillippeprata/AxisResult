using AxisTrix.Results;
using AxisTrix.Types;
using AxisTrix.Validation.Localization.Brazil;

namespace AxisTrix.Validation.Localization;

public static class LocalizationExtensions
{
    public static AxisResult<string> GetFormattedPhone(this CountryId countryId, string? phone)
    {
        if (countryId == CountryIds.Br)
        {
            var formatted = CellphoneValidator.Format(phone);
            if (formatted == null)
                return AxisError.ValidationRule("CELLPHONE_NUMBER_NULL_OR_NOT_VALID");
            return AxisResult.Ok(formatted);
        }

        return AxisError.Mapping("THERE_IS_NO_VALIDATION_FOR_THIS_COUNTRY");
    }

}
