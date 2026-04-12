using IdentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace IdentityTrix.Contracts.Authentication.v1;

public interface IAuthenticationMediator
{
    Task<AxisResult.AxisResult> AuthenticateAsync(AuthenticateExternalApiCommand command);
}
