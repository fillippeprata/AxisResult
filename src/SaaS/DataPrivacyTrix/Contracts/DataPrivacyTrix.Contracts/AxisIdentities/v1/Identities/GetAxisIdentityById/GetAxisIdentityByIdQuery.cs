using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityById;

public record GetAxisIdentityByIdQuery : IAxisQuery<GetAxisIdentityByIdResponse>
{
    public string? AxisIdentityId { get; init; }
}
