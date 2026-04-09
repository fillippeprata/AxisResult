using AxisTrix.Results;
using IdentityTrix.Domain.ExternalApis.Root;
using IdentityTrix.Ports.ExternalApis;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis;

internal interface IExternalApiAggregateApplication : IExternalApiEntityProperties
{
    Task<AxisResult> UpdateSecretAsync(string hashedSecret);
    AxisResult VerifySecret(string plainSecret);
}

internal class ExternalApiAggregateApplication(
    ExternalApiEntity root,
    IExternalApiWritePort writePort
) : ExternalApiEntity(root), IExternalApiAggregateApplication
{
    public Task<AxisResult> UpdateSecretAsync(string hashedSecret)
        => writePort.UpdateSecretAsync(root.ExternalApiId, hashedSecret);

    public AxisResult VerifySecret(string plainSecret)
    {
        if (root.ValidateSecret(plainSecret))
            return AxisResult.Ok();
        return AxisError.Unauthorized("INVALID_EXTERNAL_API_ID_OR_SECRET");
    }
}
