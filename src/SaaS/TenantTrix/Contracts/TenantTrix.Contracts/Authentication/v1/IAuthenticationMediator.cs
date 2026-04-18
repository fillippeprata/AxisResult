using Axis;
using TenantTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace TenantTrix.Contracts.Authentication.v1;

public interface IAuthenticationMediator
{
    Task<AxisResult> AuthenticateAsync(AuthenticateExternalApiCommand command);
}
