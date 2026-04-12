using AxisResult;
using IdentityTrix.Domain.ExternalApis.Root;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis;

internal interface IExternalApiAggregateApplication : IExternalApiEntityProperties
{
    Task<AxisResult.AxisResult> UpdateSecretAsync(string hashedSecret);
    AxisResult.AxisResult VerifySecret(string plainSecret);
}

internal class ExternalApiAggregateApplication(
    IExternalApiEntityProperties properties,
    IExternalApisWritePort writePort
) : ExternalApiEntity(properties), IExternalApiAggregateApplication
{
    public Task<AxisResult.AxisResult> UpdateSecretAsync(string hashedSecret)
        => writePort.UpdateSecretAsync(ExternalApiId, hashedSecret);

    public AxisResult.AxisResult VerifySecret(string plainSecret)
    {
        if (ValidateSecret(plainSecret))
            return AxisResult.AxisResult.Ok();
        return AxisError.Unauthorized("INVALID_EXTERNAL_API_ID_OR_SECRET");
    }
}
