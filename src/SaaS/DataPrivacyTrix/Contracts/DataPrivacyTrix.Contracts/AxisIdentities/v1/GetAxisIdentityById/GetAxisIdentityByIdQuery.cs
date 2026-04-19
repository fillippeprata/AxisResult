using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityById;

public record GetAxisIdentityByIdQuery : IAxisQuery<GetAxisIdentityByIdResponse>
{
    public string? AxisIdentityId { get; init; }
}
