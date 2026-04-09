using AxisTrix.Validation;
using IdentityTrix.SharedKernel.ExternalApis;
using IndentityTrix.Contracts.ExternalApis.v1.GetById;

namespace IdentityTrix.Application.ExternalApis.Validators.v1;

public class GetExternalApiByIdValidatorTrix : AxisValidatorBase<GetExternalApiByIdQuery>
{
    public GetExternalApiByIdValidatorTrix()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
