using AxisTrix.Validation;
using IdentityTrix.SharedKernel.ExternalApis;
using IdentityTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.GenerateNewExternalApiSecret;

internal class GenerateNewExternalApiSecretValidator : AxisValidatorBase<GenerateNewExternalApiSecretCommand>
{
    public GenerateNewExternalApiSecretValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
