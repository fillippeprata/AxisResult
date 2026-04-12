using AxisTrix.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Emails.v1.AddEmail;

public record AddEmailResponse : IAxisCommandResponse
{
    public required string EmailId { get; init; }
}
