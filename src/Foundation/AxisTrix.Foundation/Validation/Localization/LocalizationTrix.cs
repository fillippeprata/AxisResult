using AxisTrix.Types;
using AxisTrix.Validation.Localization.Brazil;

namespace AxisTrix.Validation.Localization;

public static class LocalizationTrix
{
    public static string? GetFormattedPhone(this CountryId countryId, string? phone)
    {
        if (countryId == CountryIds.Br)
            return CellphoneValidator.Format(phone);

        return phone;
    }
}
