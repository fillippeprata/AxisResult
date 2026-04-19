using Axis;
using DataPrivacyTrix.SharedKernel.Registration;

namespace DataPrivacyTrix.Ports.Registration;

public interface IAxisIdentitiesWritePort
{
    Task<AxisResult> CreateAsync(IAxisIdentityEntityProperties properties);
}
