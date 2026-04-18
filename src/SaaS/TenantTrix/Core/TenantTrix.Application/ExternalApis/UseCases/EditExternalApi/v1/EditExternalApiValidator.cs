using AxisValidator;
using TenantTrix.Contracts.ExternalApis.v1.EditExternalApi;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.ExternalApis.UseCases.EditExternalApi.v1;

internal class EditExternalApiValidator : AxisValidatorBase<EditExternalApiCommand>
{
    public EditExternalApiValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
        RequiredWithMaxLength(x => x.ApiName, "EXTERNAL_API_NAME_NULL_OR_TOO_LONG");
    }
}
