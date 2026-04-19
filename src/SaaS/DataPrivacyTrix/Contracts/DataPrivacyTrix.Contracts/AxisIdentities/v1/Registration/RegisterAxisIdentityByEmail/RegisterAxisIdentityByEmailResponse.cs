using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByEmail;

public record RegisterAxisIdentityByEmailResponse : IAxisCommandResponse
{
    public required string AxisIdentityId { get; init; }
}
