using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Ports.ExternalApis;

public interface IExternalApisWritePort
{
    Task<AxisResult.AxisResult> CreateAsync(IExternalApiEntityProperties properties);
    Task<AxisResult.AxisResult> UpdateSecretAsync(ExternalApiId id, string hashedSecret);
}
