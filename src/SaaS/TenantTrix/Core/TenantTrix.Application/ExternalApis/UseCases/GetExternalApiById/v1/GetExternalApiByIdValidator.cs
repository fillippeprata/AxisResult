using AxisValidator;
using TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using ExternalApiId = TenantTrix.SharedKernel.ExternalApis.ExternalApiId;

namespace TenantTrix.Application.ExternalApis.UseCases.GetExternalApiById.v1;

internal class GetExternalApiByIdValidator : AxisValidatorBase<GetExternalApiByIdQuery>
{
    public GetExternalApiByIdValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
