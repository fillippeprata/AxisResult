using DataPrivacyTrix.SharedKernel.Cellphones;

namespace DataPrivacyTrix.Ports.Cellphones;

public interface ICellphonesWritePort
{
    Task<AxisResult.AxisResult> CreateAsync(ICellphoneEntityProperties properties);
}
