using AxisTrix.Validation;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;
using DataPrivacyTrix.Domain.Cellphones.Validation;
using CountryId = AxisTrix.Types.Localization.CountryId;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetByCellphoneNumber.v1;

public class GetByCellphoneNumberValidator : AxisValidatorBase<GetByCellphoneNumberQuery>
{
    public GetByCellphoneNumberValidator()
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
