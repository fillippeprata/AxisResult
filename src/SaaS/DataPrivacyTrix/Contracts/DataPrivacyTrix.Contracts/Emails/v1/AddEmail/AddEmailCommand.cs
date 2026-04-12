using AxisTrix.CQRS.Commands;

namespace DataPrivacyTrix.Contracts.Emails.v1.AddEmail;

public record AddEmailCommand : IAxisCommand<AddEmailResponse>
{
    public string? Email { get; init; }
}
