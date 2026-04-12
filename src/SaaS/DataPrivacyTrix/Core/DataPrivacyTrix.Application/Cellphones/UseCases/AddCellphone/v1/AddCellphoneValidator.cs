using AxisTrix.Validation;
using DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;
using DataPrivacyTrix.Domain.Cellphones.Validation;
using CountryId = AxisTrix.Types.Localization.CountryId;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.AddCellphone.v1;

public class AddCellphoneValidator : AxisValidatorBase<AddCellphoneCommand>
{
    public AddCellphoneValidator()
    {
        DependentRules<CountryId, string>(
            x => (CountryId)x.CountryId,
            "COUNTRY_ID_NULL_OR_NOT_VALID",
            x => x.CellphoneNumber,
            "CELLPHONE_NUMBER_NULL_OR_NOT_VALID",
            (countryId, cellphone) => countryId.GetFormattedPhone(cellphone)
        );
    }
}
