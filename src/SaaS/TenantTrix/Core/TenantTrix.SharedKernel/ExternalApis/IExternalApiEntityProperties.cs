using Axis;

namespace TenantTrix.SharedKernel.ExternalApis;

public interface IExternalApiEntityProperties
{
    ExternalApiId ExternalApiId { get; }
    string HashedSecret { get;}
    string ApiName { get; }
    TenantId  TenantId { get; }
}
