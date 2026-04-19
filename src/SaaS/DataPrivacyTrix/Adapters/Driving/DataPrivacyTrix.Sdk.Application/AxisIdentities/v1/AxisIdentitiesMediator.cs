using Axis;
using AxisMediator.Contracts;
using DataPrivacyTrix.Contracts.AxisIdentities.v1;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Cellphones.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Emails.AddEmailToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityById;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByEmail;

namespace DataPrivacyTrix.Sdk.Application.AxisIdentities.v1;

internal class AxisIdentitiesMediator(IAxisMediator mediator) : IAxisIdentitiesMediator
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
