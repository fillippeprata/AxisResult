using Axis;
using AxisMediator.Contracts;
using DataPrivacyTrix.Contracts.AxisIdentities.v1;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.AddEmailToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityById;

namespace DataPrivacyTrix.Sdk.Application.AxisIdentities.v1;

internal class AxisIdentitiesMediator(IAxisMediator mediator) : IAxisIdentitiesMediator
{
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
