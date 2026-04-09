using IdentityTrix.SharedKernel.ExternalApis;

namespace IdentityTrix.Domain.ExternalApis.Root;

internal partial class ExternalApiEntity(
    ExternalApiId externalApiId,
    string hashedSecret,
    string apiName)
    : IExternalApiEntityProperties
{
    #region Properties

    public ExternalApiId ExternalApiId { get; } = externalApiId;
    public string HashedSecret { get; } = hashedSecret;
    public string ApiName { get; } = apiName;

    internal ExternalApiEntity(IExternalApiEntityProperties properties) : this(
        properties.ExternalApiId,
        properties.HashedSecret,
        properties.ApiName
    ) {}

    #endregion
}
