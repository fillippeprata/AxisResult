using Axis;
using DataPrivacyTrix.Domain.Cellphones.Root;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones;

internal interface ICellphoneAggregateApplication : ICellphoneEntityProperties
{
    Task<AxisResult> IsValidAsync();
}

internal class CellphoneAggregateApplication(
    ICellphoneEntityProperties properties
) : CellphoneEntity(properties), ICellphoneAggregateApplication
{
    public new Task<AxisResult> IsValidAsync()
        => base.IsValidAsync();
}
