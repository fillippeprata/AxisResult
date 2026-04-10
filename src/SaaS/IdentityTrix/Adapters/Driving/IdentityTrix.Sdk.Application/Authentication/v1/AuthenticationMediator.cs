using AxisTrix;
using AxisTrix.Results;
using IdentityTrix.Contracts.Authentication.v1;
using IdentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace IdentityTrix.Sdk.Application.Authentication.v1;

internal class AuthenticationMediator(IAxisMediator mediator) : IAuthenticationMediator
{
    public Task<AxisResult> AuthenticateAsync(AuthenticateExternalApiCommand command)
        => mediator.Cqrs.ExecuteAsync(command);
}
