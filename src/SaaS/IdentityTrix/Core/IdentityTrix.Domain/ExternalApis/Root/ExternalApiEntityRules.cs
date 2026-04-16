using Axis;
using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Domain.ExternalApis.Root;

internal partial class ExternalApiEntity
{
    public AxisResult ValidateSecret(string plainSecret)
        => ExternalApiSecret.Verify(plainSecret, HashedSecret)
            ? AxisResult.Ok()
            : AxisError.Unauthorized("INVALID_EXTERNAL_API_ID_OR_SECRET");
}
