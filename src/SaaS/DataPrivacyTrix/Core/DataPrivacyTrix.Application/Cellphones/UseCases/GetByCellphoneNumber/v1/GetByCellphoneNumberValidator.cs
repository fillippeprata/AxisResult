using AxisValidator;
using AxisValidator.Brazil;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetByCellphoneNumber.v1;

internal class GetByCellphoneNumberValidator : AxisValidatorBase<GetByCellphoneNumberQuery>
{
    public GetByCellphoneNumberValidator()
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
