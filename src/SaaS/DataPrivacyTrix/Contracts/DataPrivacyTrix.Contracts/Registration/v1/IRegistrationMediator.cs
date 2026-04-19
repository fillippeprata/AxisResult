using Axis;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;

namespace DataPrivacyTrix.Contracts.Registration.v1;

public interface IRegistrationMediator
{
    Task<AxisResult<RegisterAxisIdentityByCellphoneResponse>> RegisterByCellphoneAsync(RegisterAxisIdentityByCellphoneCommand command);
    Task<AxisResult<RegisterAxisIdentityByEmailResponse>> RegisterByEmailAsync(RegisterAxisIdentityByEmailCommand command);
}
