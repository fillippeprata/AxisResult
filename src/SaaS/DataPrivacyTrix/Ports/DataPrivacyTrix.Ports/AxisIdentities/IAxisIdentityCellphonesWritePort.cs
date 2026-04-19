using Axis;
using DataPrivacyTrix.SharedKernel.Cellphones;
using AxisIdentityId = DataPrivacyTrix.SharedKernel.AxisIdentities.AxisIdentityId;

namespace DataPrivacyTrix.Ports.AxisIdentities;

public interface IAxisIdentityCellphonesWritePort
{
    Task<AxisResult> AddCellphoneAsync(AxisIdentityId axisIdentityId, CellphoneId cellphoneId);
}
