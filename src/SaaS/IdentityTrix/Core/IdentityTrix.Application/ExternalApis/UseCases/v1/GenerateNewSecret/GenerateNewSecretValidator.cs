using AxisTrix.Validation;
using IdentityTrix.SharedKernel.ExternalApis;
using IdentityTrix.Contracts.ExternalApis.v1.GenerateNewSecret;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.GenerateNewSecret;

internal class GenerateNewSecretValidator : AxisValidatorBase<GenerateNewExternalApiSecretCommand>
{
    public GenerateNewSecretValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
