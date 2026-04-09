using AxisTrix.Results;
using IndentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace IndentityTrix.Contracts.Authentication.v1;

public interface IAuthenticationMediator
{
    Task<AxisResult> AuthenticateAsync(AuthenticateExternalApiCommand command);
}
