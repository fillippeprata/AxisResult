using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;

public record RegisterAxisIdentityByEmailResponse : IAxisCommandResponse
{
    public required string AxisIdentityId { get; init; }
}
