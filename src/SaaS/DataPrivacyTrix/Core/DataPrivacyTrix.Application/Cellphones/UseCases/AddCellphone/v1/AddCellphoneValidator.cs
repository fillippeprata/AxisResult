using AxisValidator;
using AxisValidator.Brazil;
using DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.AddCellphone.v1;

internal class AddCellphoneValidator : AxisValidatorBase<AddCellphoneCommand>
{
    public AddCellphoneValidator()
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
