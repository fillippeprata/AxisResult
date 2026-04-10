using AxisTrix.Validation;
using IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;

namespace IdentityTrix.Application.ExternalApis.UseCases.v1.AddExternalApi;

internal class AddExternalApiValidator : AxisValidatorBase<AddExternalApiCommand>
{
    public AddExternalApiValidator()
    {
        RequiredWithMaxLength(x => x.ApiName, "EXTERNAL_API_NAME_NULL_OR_TOO_LONG");
    }
}
