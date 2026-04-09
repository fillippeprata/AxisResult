using AxisTrix.Validation;
using IndentityTrix.Contracts.ExternalApis.v1.AddExternalApi;

namespace IdentityTrix.Application.ExternalApis.Validators.v1;

public class AddExternalApiValidatorTrix : AxisValidatorBase<AddExternalApiCommand>
{
    public AddExternalApiValidatorTrix()
    {
        RequiredWithMaxLenght(x => x.ApiName, "EXTERNAL_API_NAME_NULL_OR_TOO_LONG");
    }
}
