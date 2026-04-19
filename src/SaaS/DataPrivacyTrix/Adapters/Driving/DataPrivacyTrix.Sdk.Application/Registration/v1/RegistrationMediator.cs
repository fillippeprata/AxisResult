using Axis;
using AxisMediator.Contracts;
using DataPrivacyTrix.Contracts.Registration.v1;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;

namespace DataPrivacyTrix.Sdk.Application.Registration.v1;

internal class RegistrationMediator(IAxisMediator mediator) : IRegistrationMediator
{
    public Task<AxisResult<RegisterAxisIdentityByCellphoneResponse>> RegisterByCellphoneAsync(RegisterAxisIdentityByCellphoneCommand command)
        => mediator.Cqrs.ExecuteAsync<RegisterAxisIdentityByCellphoneCommand, RegisterAxisIdentityByCellphoneResponse>(command);

    public Task<AxisResult<RegisterAxisIdentityByEmailResponse>> RegisterByEmailAsync(RegisterAxisIdentityByEmailCommand command)
        => mediator.Cqrs.ExecuteAsync<RegisterAxisIdentityByEmailCommand, RegisterAxisIdentityByEmailResponse>(command);
}
