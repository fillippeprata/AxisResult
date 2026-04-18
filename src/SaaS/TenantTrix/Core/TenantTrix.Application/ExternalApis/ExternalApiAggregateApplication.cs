using Axis;
using TenantTrix.Domain.ExternalApis.Root;
using TenantTrix.Ports.ExternalApis;
using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.Application.ExternalApis;

internal interface IExternalApiAggregateApplication : IExternalApiEntityProperties
{
    Task<AxisResult> UpdateSecretAsync(string hashedSecret);
    AxisResult VerifySecret(string plainSecret);
}

internal class ExternalApiAggregateApplication(
    IExternalApiEntityProperties properties,
    IExternalApisWritePort writePort
) : ExternalApiEntity(properties), IExternalApiAggregateApplication
{
    public Task<AxisResult> UpdateSecretAsync(string hashedSecret)
        => writePort.UpdateSecretAsync(ExternalApiId, hashedSecret);

    public AxisResult VerifySecret(string plainSecret)
        => ValidateSecret(plainSecret);
}
