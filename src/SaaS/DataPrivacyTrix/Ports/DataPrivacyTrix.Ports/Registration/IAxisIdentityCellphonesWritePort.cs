using Axis;
using DataPrivacyTrix.SharedKernel.Cellphones;
using DataPrivacyTrix.SharedKernel.Registration;

namespace DataPrivacyTrix.Ports.Registration;

public interface IAxisIdentityCellphonesWritePort
{
    Task<AxisResult> AddCellphoneAsync(AxisIdentityId axisIdentityId, CellphoneId cellphoneId);
}
