using Axis;
using AxisMediator.Contracts;
using DataPrivacyTrix.Contracts.Registration.v1;
using DataPrivacyTrix.Contracts.Registration.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.Registration.v1.AddEmailToAxisIdentity;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityById;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;

namespace DataPrivacyTrix.Sdk.Application.Registration.v1;

internal class RegistrationMediator(IAxisMediator mediator) : IRegistrationMediator
{
    public Task<AxisResult<RegisterAxisIdentityByCellphoneResponse>> RegisterByCellphoneAsync(RegisterAxisIdentityByCellphoneCommand command)
        => mediator.Cqrs.ExecuteAsync<RegisterAxisIdentityByCellphoneCommand, RegisterAxisIdentityByCellphoneResponse>(command);

    public Task<AxisResult<RegisterAxisIdentityByEmailResponse>> RegisterByEmailAsync(RegisterAxisIdentityByEmailCommand command)
        => mediator.Cqrs.ExecuteAsync<RegisterAxisIdentityByEmailCommand, RegisterAxisIdentityByEmailResponse>(command);

    public Task<AxisResult> AddCellphoneAsync(AddCellphoneToAxisIdentityCommand command)
        => mediator.Cqrs.ExecuteAsync(command);

    public Task<AxisResult> AddEmailAsync(AddEmailToAxisIdentityCommand command)
        => mediator.Cqrs.ExecuteAsync(command);

    public Task<AxisResult<GetAxisIdentityByIdResponse>> GetByIdAsync(GetAxisIdentityByIdQuery query)
        => mediator.Cqrs.QueryAsync<GetAxisIdentityByIdQuery, GetAxisIdentityByIdResponse>(query);

    public Task<AxisResult<GetAxisIdentityByCellphoneResponse>> GetByCellphoneAsync(GetAxisIdentityByCellphoneQuery query)
        => mediator.Cqrs.QueryAsync<GetAxisIdentityByCellphoneQuery, GetAxisIdentityByCellphoneResponse>(query);

    public Task<AxisResult<GetAxisIdentityByEmailResponse>> GetByEmailAsync(GetAxisIdentityByEmailQuery query)
        => mediator.Cqrs.QueryAsync<GetAxisIdentityByEmailQuery, GetAxisIdentityByEmailResponse>(query);
}
