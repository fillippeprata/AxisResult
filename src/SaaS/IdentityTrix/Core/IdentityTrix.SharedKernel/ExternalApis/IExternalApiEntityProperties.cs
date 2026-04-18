using Axis;

namespace IdentityTrix.SharedKernel.ExternalApis;

public interface IExternalApiEntityProperties
{
    ExternalApiId ExternalApiId { get; }
    string HashedSecret { get;}
    string ApiName { get; }
}
