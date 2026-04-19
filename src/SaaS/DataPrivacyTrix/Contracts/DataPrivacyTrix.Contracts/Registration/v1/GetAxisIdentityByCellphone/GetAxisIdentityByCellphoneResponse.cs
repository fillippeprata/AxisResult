using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByCellphone;

public record GetAxisIdentityByCellphoneResponse : IAxisQueryResponse
{
    public required string AxisIdentityId { get; init; }
    public required string DisplayName { get; init; }
}
