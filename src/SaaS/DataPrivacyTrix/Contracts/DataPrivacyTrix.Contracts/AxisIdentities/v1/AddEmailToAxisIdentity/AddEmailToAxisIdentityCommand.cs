using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.AddEmailToAxisIdentity;

public record AddEmailToAxisIdentityCommand : IAxisCommand
{
    public string? AxisIdentityId { get; init; }
    public string? EmailId { get; init; }
}
