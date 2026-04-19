using Axis;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.AddEmailToAxisIdentity;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityById;

namespace DataPrivacyTrix.Contracts.AxisIdentities.v1;

public interface IAxisIdentitiesMediator
{
    Task<AxisResult> AddCellphoneAsync(AddCellphoneToAxisIdentityCommand command);
    Task<AxisResult> AddEmailAsync(AddEmailToAxisIdentityCommand command);
    Task<AxisResult<GetAxisIdentityByIdResponse>> GetByIdAsync(GetAxisIdentityByIdQuery query);
    Task<AxisResult<GetAxisIdentityByCellphoneResponse>> GetByCellphoneAsync(GetAxisIdentityByCellphoneQuery query);
    Task<AxisResult<GetAxisIdentityByEmailResponse>> GetByEmailAsync(GetAxisIdentityByEmailQuery query);
}
