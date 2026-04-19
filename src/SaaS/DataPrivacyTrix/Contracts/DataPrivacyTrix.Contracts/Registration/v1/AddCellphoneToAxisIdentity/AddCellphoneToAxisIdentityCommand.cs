using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Registration.v1.AddCellphoneToAxisIdentity;

public record AddCellphoneToAxisIdentityCommand : IAxisCommand
{
    public string? AxisIdentityId { get; init; }
    public string? CellphoneId { get; init; }
}
