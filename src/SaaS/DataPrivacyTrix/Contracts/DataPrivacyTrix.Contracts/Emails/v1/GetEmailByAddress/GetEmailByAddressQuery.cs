using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Emails.v1.GetEmailByAddress;

public record GetEmailByAddressQuery : IAxisQuery<GetEmailByAddressResponse>
{
    public string? Email { get; init; }
}
