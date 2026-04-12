using AxisTrix.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;

public record GetByEmailAddressQuery : IAxisQuery<GetByEmailAddressResponse>
{
    public string? Email { get; init; }
}
