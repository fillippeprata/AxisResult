using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByEmail;

public record GetAxisIdentityByEmailQuery : IAxisQuery<GetAxisIdentityByEmailResponse>
{
    public string? EmailAddress { get; init; }
}
