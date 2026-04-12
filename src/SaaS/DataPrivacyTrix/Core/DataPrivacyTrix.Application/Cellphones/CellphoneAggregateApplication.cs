using DataPrivacyTrix.Domain.Cellphones.Root;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Application.Cellphones;

internal interface ICellphoneAggregateApplication : ICellphoneEntityProperties;

internal class CellphoneAggregateApplication(
    ICellphoneEntityProperties properties
) : CellphoneEntity(properties), ICellphoneAggregateApplication;
