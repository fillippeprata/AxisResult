using AxisValidator;
using TenantTrix.Contracts.ExternalApis.v1.AuthenticateExternalApi;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.ExternalApis.UseCases.AuthenticateExternalApi.v1;

internal class AuthenticateExternalApiValidator : AxisValidatorBase<AuthenticateExternalApiCommand>
{
    public AuthenticateExternalApiValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
        NotNullOrEmpty(x => x.Secret, "EXTERNAL_API_SECRET_NULL_OR_EMPTY");
    }
}
