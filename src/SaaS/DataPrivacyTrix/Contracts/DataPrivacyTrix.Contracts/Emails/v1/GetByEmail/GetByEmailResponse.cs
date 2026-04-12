using AxisTrix.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Emails.v1.GetByEmail;

public record GetByEmailResponse : IAxisQueryResponse
{
    public required string EmailId { get; init; }
}
