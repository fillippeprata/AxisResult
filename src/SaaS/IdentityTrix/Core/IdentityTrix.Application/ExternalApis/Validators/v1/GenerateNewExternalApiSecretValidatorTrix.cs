using AxisTrix.Validation;
using IdentityTrix.SharedKernel.ExternalApis;
using IndentityTrix.Contracts.ExternalApis.v1.GenerateNewSecret;

namespace IdentityTrix.Application.ExternalApis.Validators.v1;

public class GenerateNewExternalApiSecretValidatorTrix : AxisValidatorBase<GenerateNewExternalApiSecretCommand>
{
    public GenerateNewExternalApiSecretValidatorTrix()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
