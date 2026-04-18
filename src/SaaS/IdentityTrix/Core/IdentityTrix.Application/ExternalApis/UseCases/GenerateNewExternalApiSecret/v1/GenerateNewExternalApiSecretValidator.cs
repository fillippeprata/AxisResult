using AxisValidator.FluentValidation;
using IdentityTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis.UseCases.GenerateNewExternalApiSecret.v1;

internal class GenerateNewExternalApiSecretValidator : AxisValidatorBase<GenerateNewExternalApiSecretCommand>
{
    public GenerateNewExternalApiSecretValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
