using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByEmail;

public record GetAxisIdentityByEmailResponse : IAxisQueryResponse
{
    public required string AxisIdentityId { get; init; }
    public required string DisplayName { get; init; }
}
