using AxisTrix.Types;
using AxisTrix.Validation;
using DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;

namespace DataPrivacyTrix.Application.Cellphones.UseCases.AddCellphone.v1;

public class AddCellphoneValidator : AxisValidatorBase<AddCellphoneCommand>
{
    public AddCellphoneValidator()
    {
        NotNullOrEmpty(x => CountryIds.GetById(x.CountryId), "COUNTRY_ID_NULL_OR_NOT_VALID",
            countryId => RequiredCellPhone(x => x.CellphoneNumber, countryId!.Value, "CELLPHONE_NUMBER_NULL_OR_NOT_VALID"));
    }
}
