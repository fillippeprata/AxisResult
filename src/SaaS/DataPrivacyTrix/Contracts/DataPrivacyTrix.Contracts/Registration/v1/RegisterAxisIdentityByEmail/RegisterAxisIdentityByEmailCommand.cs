using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.Registration.v1.SharedData;

namespace DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;

public record RegisterAxisIdentityByEmailCommand : IAxisCommand<RegisterAxisIdentityByEmailResponse>
{
    public RegisterAxisIdentityData? Data { get; init; }
    public string? EmailId { get; init; }
}
