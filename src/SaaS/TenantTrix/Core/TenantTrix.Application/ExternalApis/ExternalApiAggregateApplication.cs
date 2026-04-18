using Axis;
using TenantTrix.Domain.ExternalApis.Root;
using TenantTrix.Ports.ExternalApis;
using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.Application.ExternalApis;

internal interface IExternalApiAggregateApplication : IExternalApiEntityProperties
{
    Task<AxisResult> UpdateSecretAsync(string hashedSecret);
    Task<AxisResult> UpdateNameAsync(string apiName);
    AxisResult VerifySecret(string plainSecret);
}

internal class ExternalApiAggregateApplication(
    IExternalApiEntityProperties properties,
    IExternalApisReaderPort readerPort,
    IExternalApisWritePort writePort
) : ExternalApiEntity(properties), IExternalApiAggregateApplication
{
    public Task<AxisResult> UpdateSecretAsync(string hashedSecret)
        => writePort.UpdateSecretAsync(ExternalApiId, hashedSecret);

    public Task<AxisResult> UpdateNameAsync(string apiName)
        => readerPort.GetByNameAsync(apiName)
            .EnsureAsync(
                existing => existing.ExternalApiId == ExternalApiId,
                AxisError.ValidationRule("EXTERNAL_API_NAME_ALREADY_EXISTS"))
            .RecoverNotFoundAsync(() => this)
            .ThenAsync(_ => writePort.UpdateNameAsync(ExternalApiId, apiName))
            .MatchAsync(onSuccess: _ => AxisResult.Ok(), AxisResult.Error);

    public AxisResult VerifySecret(string plainSecret)
        => ValidateSecret(plainSecret);
}
