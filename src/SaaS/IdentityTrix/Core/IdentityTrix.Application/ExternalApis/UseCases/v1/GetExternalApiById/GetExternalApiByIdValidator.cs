using AxisValidator.FluentValidation;
using IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.GetExternalApiById;

internal class GetExternalApiByIdValidator : AxisValidatorBase<GetExternalApiByIdQuery>
{
    public GetExternalApiByIdValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NULL_OR_NOT_GUID_7", x => ExternalApiId.TryParse(x, out _));
    }
}
