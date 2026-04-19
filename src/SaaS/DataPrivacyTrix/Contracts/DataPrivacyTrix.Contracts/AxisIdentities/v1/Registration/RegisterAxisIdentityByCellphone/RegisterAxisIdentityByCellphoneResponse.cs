using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByCellphone;

public record RegisterAxisIdentityByCellphoneResponse : IAxisCommandResponse
{
    public required string AxisIdentityId { get; init; }
}
