using Axis;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Cellphones.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Emails.AddEmailToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityById;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Registration.RegisterAxisIdentityByEmail;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1;

public interface IAxisIdentitiesMediator
{
    Task<AxisResult> AddCellphoneAsync(AddCellphoneToAxisIdentityCommand command);
    Task<AxisResult> AddEmailAsync(AddEmailToAxisIdentityCommand command);
    Task<AxisResult<GetAxisIdentityByIdResponse>> GetByIdAsync(GetAxisIdentityByIdQuery query);
    Task<AxisResult<GetAxisIdentityByCellphoneResponse>> GetByCellphoneAsync(GetAxisIdentityByCellphoneQuery query);
    Task<AxisResult<GetAxisIdentityByEmailResponse>> GetByEmailAsync(GetAxisIdentityByEmailQuery query);
    Task<AxisResult<RegisterAxisIdentityByCellphoneResponse>> RegisterByCellphoneAsync(RegisterAxisIdentityByCellphoneCommand command);
    Task<AxisResult<RegisterAxisIdentityByEmailResponse>> RegisterByEmailAsync(RegisterAxisIdentityByEmailCommand command);
}
