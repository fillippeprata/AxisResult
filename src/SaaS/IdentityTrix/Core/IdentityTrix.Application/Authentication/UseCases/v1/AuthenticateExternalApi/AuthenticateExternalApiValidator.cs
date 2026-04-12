using AxisTrix.Validation;
using IdentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.Authentication.UseCases.v1.AuthenticateExternalApi;

internal class AuthenticateExternalApiValidator : AxisValidatorBase<AuthenticateExternalApiCommand>
{
    public AuthenticateExternalApiValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
        NotNullOrEmpty(x => x.Secret, "EXTERNAL_API_SECRET_NULL_OR_EMPTY");
    }
}
