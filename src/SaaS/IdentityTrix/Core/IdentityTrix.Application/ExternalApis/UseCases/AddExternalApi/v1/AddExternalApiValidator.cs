using AxisValidator.FluentValidation;
using IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;

namespace IdentityTrix.Application.ExternalApis.UseCases.AddExternalApi.v1;

internal class AddExternalApiValidator : AxisValidatorBase<AddExternalApiCommand>
{
    public AddExternalApiValidator()
    {
        RequiredWithMaxLength(x => x.ApiName, "EXTERNAL_API_NAME_NULL_OR_TOO_LONG");
    }
}
