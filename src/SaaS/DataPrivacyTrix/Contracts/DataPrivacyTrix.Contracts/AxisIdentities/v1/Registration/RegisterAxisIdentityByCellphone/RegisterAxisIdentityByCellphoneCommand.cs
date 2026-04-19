using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.SharedData;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByCellphone;

public record RegisterAxisIdentityByCellphoneCommand : IAxisCommand<RegisterAxisIdentityByCellphoneResponse>
{
    public RegisterAxisIdentityData? Data { get; init; }
    public string? CellphoneId { get; init; }
}
