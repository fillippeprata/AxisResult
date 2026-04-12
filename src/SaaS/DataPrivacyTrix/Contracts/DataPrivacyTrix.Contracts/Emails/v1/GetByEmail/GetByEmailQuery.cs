using AxisTrix.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Emails.v1.GetByEmail;

public record GetByEmailQuery : IAxisQuery<GetByEmailResponse>
{
    public string? Email { get; init; }
}
