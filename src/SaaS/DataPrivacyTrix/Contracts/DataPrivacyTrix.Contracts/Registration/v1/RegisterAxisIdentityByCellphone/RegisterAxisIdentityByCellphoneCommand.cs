using AxisMediator.Contracts.CQRS.Commands;
using DataPrivacyTrix.Contracts.Registration.v1.SharedData;

namespace DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;

public record RegisterAxisIdentityByCellphoneCommand : IAxisCommand<RegisterAxisIdentityByCellphoneResponse>
{
    public RegisterAxisIdentityData? Data { get; init; }
    public string? CellphoneId { get; init; }
}
