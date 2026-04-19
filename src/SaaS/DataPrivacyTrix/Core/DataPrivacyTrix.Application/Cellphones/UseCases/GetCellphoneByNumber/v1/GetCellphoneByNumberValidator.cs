using AxisValidator;
using AxisValidator.Brazil;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetCellphoneByNumber;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetCellphoneByNumber.v1;

internal class GetCellphoneByNumberValidator : AxisValidatorBase<GetCellphoneByNumberQuery>
{
    public GetCellphoneByNumberValidator()
    {
        DependentRules<CountryId, string>(
            x => (CountryId)x.CountryId,
            "COUNTRY_ID_NULL_OR_NOT_VALID",
            x => x.CellphoneNumber,
            "CELLPHONE_NUMBER_NULL_OR_NOT_VALID",
            (countryId, cellphone) => countryId.FormatCellphone(cellphone)
        );
    }
}
