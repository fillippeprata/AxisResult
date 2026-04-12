using DataPrivacyTrix.Domain.Cellphones.Validation;

namespace DataPrivacyTrix.Domain.Cellphones.Root;

internal partial class CellphoneEntity
{
    protected Task<AxisResult.AxisResult> IsValidAsync()
        => Task.FromResult<AxisResult.AxisResult>(CountryId.GetFormattedPhone(CellphoneNumber));
}
