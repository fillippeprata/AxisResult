using AxisMediator.Contracts.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Emails.AddEmailToAxisIdentity;

public record AddEmailToAxisIdentityCommand : IAxisCommand
{
    public string? AxisIdentityId { get; init; }
    public string? EmailId { get; init; }
}
