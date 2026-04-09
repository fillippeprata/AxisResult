using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Domain.ExternalApis.Root;

internal partial class ExternalApiEntity
{
    public bool ValidateSecret(string plainSecret)
        => ExternalApiSecret.Verify(plainSecret, HashedSecret);
}
