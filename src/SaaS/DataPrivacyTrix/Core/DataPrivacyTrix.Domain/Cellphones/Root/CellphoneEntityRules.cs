using AxisTrix.Results;
using DataPrivacyTrix.Domain.Cellphones.Validation;

namespace DataPrivacyTrix.Domain.Cellphones.Root;

internal partial class CellphoneEntity
{
    protected Task<AxisResult> IsValidAsync()
        => Task.FromResult<AxisResult>(CountryId.GetFormattedPhone(CellphoneNumber));
}
