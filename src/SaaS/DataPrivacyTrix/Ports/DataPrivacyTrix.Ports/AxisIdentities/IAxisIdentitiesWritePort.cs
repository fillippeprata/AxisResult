using Axis;
using DataPrivacyTrix.SharedKernel.AxisIdentities;

namespace DataPrivacyTrix.Ports.AxisIdentities;

public interface IAxisIdentitiesWritePort
{
    Task<AxisResult> CreateAsync(IAxisIdentityEntityProperties properties);
}
