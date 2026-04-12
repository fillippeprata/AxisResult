using AxisTrix.Types;
using AxisTrix.Validation;
using DataPrivacyTrix.Contracts.Cellphones.v1.GetByCellphoneNumber;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.GetByCellphoneNumber.v1;

public class GetByCellphoneNumberValidator : AxisValidatorBase<GetByCellphoneNumberQuery>
{
    public GetByCellphoneNumberValidator()
    {
        NotNullOrEmpty(x => CountryIds.GetById(x.CountryId), "COUNTRY_ID_NULL_OR_NOT_VALID",
            countryId => RequiredCellPhone(x => x.CellphoneNumber, countryId!.Value, "CELLPHONE_NUMBER_NULL_OR_NOT_VALID"));
    }
}
