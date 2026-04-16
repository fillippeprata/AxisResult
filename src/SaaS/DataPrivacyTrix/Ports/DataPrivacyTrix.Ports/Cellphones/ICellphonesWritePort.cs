using Axis;
using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Ports.Cellphones;

public interface ICellphonesWritePort
{
    Task<AxisResult> CreateAsync(ICellphoneEntityProperties properties);
}
