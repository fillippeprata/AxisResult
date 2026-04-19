using Axis;
using DataPrivacyTrix.Contracts.Registration.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.Registration.v1.AddEmailToAxisIdentity;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityById;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;

namespace DataPrivacyTrix.Contracts.Registration.v1;

public interface IRegistrationMediator
{
    Task<AxisResult<RegisterAxisIdentityByCellphoneResponse>> RegisterByCellphoneAsync(RegisterAxisIdentityByCellphoneCommand command);
    Task<AxisResult<RegisterAxisIdentityByEmailResponse>> RegisterByEmailAsync(RegisterAxisIdentityByEmailCommand command);
    Task<AxisResult> AddCellphoneAsync(AddCellphoneToAxisIdentityCommand command);
    Task<AxisResult> AddEmailAsync(AddEmailToAxisIdentityCommand command);
    Task<AxisResult<GetAxisIdentityByIdResponse>> GetByIdAsync(GetAxisIdentityByIdQuery query);
    Task<AxisResult<GetAxisIdentityByCellphoneResponse>> GetByCellphoneAsync(GetAxisIdentityByCellphoneQuery query);
    Task<AxisResult<GetAxisIdentityByEmailResponse>> GetByEmailAsync(GetAxisIdentityByEmailQuery query);
}
