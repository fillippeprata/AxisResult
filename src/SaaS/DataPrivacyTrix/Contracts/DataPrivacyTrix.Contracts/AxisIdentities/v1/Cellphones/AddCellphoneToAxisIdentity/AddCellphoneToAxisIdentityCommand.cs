using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Cellphones.AddCellphoneToAxisIdentity;

public record AddCellphoneToAxisIdentityCommand : IAxisCommand
{
    public string? AxisIdentityId { get; init; }
    public string? CellphoneId { get; init; }
}
