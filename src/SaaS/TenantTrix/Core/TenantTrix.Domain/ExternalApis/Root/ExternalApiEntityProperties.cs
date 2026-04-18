using Axis;
using TenantTrix.SharedKernel.ExternalApis;

namespace TenantTrix.Domain.ExternalApis.Root;

internal partial class ExternalApiEntity(
    ExternalApiId externalApiId,
    string hashedSecret,
    string apiName,
    TenantId tenantId)
    : IExternalApiEntityProperties
{
    #region Properties

    public ExternalApiId ExternalApiId { get; } = externalApiId;
    public string HashedSecret { get; } = hashedSecret;
    public string ApiName { get; } = apiName;
    public TenantId TenantId { get; } = tenantId;

    internal ExternalApiEntity(IExternalApiEntityProperties properties) : this(
        properties.ExternalApiId,
        properties.HashedSecret,
        properties.ApiName,
        properties.TenantId
    ) {}

    #endregion
}
