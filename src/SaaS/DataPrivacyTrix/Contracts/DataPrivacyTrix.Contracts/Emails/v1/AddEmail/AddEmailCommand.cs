using AxisTrix.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Emails.v1.AddEmail;

public record AddEmailCommand : IAxisCommand<AddEmailResponse>
{
    public string? EmailAddress { get; init; }
}
