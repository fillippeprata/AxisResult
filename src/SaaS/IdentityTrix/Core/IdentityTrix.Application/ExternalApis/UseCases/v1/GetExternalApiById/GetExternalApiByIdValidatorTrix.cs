using AxisTrix.Validation;
using IdentityTrix.SharedKernel.ExternalApis;
using IndentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.GetExternalApiById;

public class GetExternalApiByIdValidatorTrix : AxisValidatorBase<GetExternalApiByIdQuery>
{
    public GetExternalApiByIdValidatorTrix()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
