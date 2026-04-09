using AxisTrix.Results;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Ports.ExternalApis;

public interface IExternalApiReaderPort
{
    Task<AxisResult<IExternalApiEntityProperties>> GetExternalApiByIdAsync(ExternalApiId id);
}
