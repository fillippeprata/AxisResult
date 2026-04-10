using AxisTrix.Validation;
using IdentityTrix.SharedKernel.ExternalApis;
using IndentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace IdentityTrix.Application.Authentication.UseCases.v1.AuthenticateExternalApi;

public class AuthenticateExternalApiValidatorTrix : AxisValidatorBase<AuthenticateExternalApiCommand>
{
    public AuthenticateExternalApiValidatorTrix()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
        NotNullOrEmpty(x => x.Secret, "EXTERNAL_API_SECRET_NULL_OR_EMPTY");
    }
}
