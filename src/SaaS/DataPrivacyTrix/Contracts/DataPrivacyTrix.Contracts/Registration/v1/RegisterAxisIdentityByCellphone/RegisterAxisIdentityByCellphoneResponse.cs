using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;

public record RegisterAxisIdentityByCellphoneResponse : IAxisCommandResponse
{
    public required string AxisIdentityId { get; init; }
}
