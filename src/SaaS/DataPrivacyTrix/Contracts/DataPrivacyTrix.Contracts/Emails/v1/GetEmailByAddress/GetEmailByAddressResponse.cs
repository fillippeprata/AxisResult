using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Emails.v1.GetEmailByAddress;

public record GetEmailByAddressResponse : IAxisQueryResponse
{
    public required string EmailId { get; init; }
}
