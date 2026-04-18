using Axis;
using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.Ports.ExternalApis;

public interface IExternalApisReaderPort
{
    Task<AxisResult<IExternalApiEntityProperties>> GetByIdAsync(ExternalApiId id);
    Task<AxisResult<IExternalApiEntityProperties>> GetByNameAsync(string apiName);
}
