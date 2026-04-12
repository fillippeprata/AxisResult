using AxisTrix;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Ports.ExternalApis;

public interface IExternalApisWritePort
{
    Task<AxisResult> CreateAsync(IExternalApiEntityProperties properties);
    Task<AxisResult> UpdateSecretAsync(ExternalApiId id, string hashedSecret);
}
