using AxisMediator.Contracts.CQRS.Queries;

namespace DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByEmail;

public record GetAxisIdentityByEmailQuery : IAxisQuery<GetAxisIdentityByEmailResponse>
{
    public string? EmailAddress { get; init; }
}
