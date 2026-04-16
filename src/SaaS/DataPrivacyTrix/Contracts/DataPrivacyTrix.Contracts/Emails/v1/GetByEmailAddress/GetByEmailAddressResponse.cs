using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;

public record GetByEmailAddressResponse : IAxisQueryResponse
{
    public required string EmailId { get; init; }
}
