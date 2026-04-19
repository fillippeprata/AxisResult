using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByCellphone;

public record GetAxisIdentityByCellphoneResponse : IAxisQueryResponse
{
    public required string AxisIdentityId { get; init; }
    public required string DisplayName { get; init; }
}
