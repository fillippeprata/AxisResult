using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.SharedData;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByEmail;

public record RegisterAxisIdentityByEmailCommand : IAxisCommand<RegisterAxisIdentityByEmailResponse>
{
    public RegisterAxisIdentityData? Data { get; init; }
    public string? EmailId { get; init; }
}
