using AxisTrix;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Ports.ExternalApis;

public interface IExternalApisReaderPort
{
    Task<AxisResult<IExternalApiEntityProperties>> GetByIdAsync(ExternalApiId id);
}
