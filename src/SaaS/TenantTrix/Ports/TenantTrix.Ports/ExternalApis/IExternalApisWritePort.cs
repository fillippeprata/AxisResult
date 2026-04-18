using Axis;
using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.Ports.ExternalApis;

public interface IExternalApisWritePort
{
    Task<AxisResult> CreateAsync(IExternalApiEntityProperties properties);
    Task<AxisResult> UpdateSecretAsync(ExternalApiId id, string hashedSecret);
    Task<AxisResult> UpdateNameAsync(ExternalApiId id, string apiName);
}
