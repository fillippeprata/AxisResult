using AxisValidator;
using TenantTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.ExternalApis.UseCases.GenerateNewExternalApiSecret.v1;

internal class GenerateNewExternalApiSecretValidator : AxisValidatorBase<GenerateNewExternalApiSecretCommand>
{
    public GenerateNewExternalApiSecretValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
