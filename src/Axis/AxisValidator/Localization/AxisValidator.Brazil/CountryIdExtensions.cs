using Axis;
using Axis.Localization;
using CountryId = Axis.Localization.CountryId;

namespace AxisValidator.Brazil;

public static class CountryIdExtensions
{
    public static AxisResult<string> FormatCellphone(this CountryId countryId, string? phone)
    {
        if (countryId != CountryIds.Br)
            return AxisError.Mapping("IT_IS_NOT_BRAZIL_COUNTRY");

        var formatted = CellphoneValidator.Format(phone);
        if (formatted == null)
            return AxisError.ValidationRule("CELLPHONE_NUMBER_NULL_OR_NOT_VALID");

        return AxisResult.Ok(formatted);

    }

    public static AxisResult<string> ValidateDocument(this CountryId countryId, bool isIndividual, string? document)
    {
        if (countryId != CountryIds.Br || !isIndividual)
            return AxisError.BusinessRule("COUNTRY_ID_DOCUMENT_NOT_IMPLEMENTED");

        if (!CpfValidator.Validate(document))
            return AxisError.ValidationRule("DOCUMENT_INVALID");

        return AxisResult.Ok(document!);
    }

}
