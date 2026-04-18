using Axis;
using AxisMediator.Contracts;
using TenantTrix.Contracts.Authentication.v1;
using TenantTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

namespace TenantTrix.Sdk.Application.Authentication.v1;

internal class AuthenticationMediator(IAxisMediator mediator) : IAuthenticationMediator
{
    public Task<AxisResult> AuthenticateAsync(AuthenticateExternalApiCommand command)
        => mediator.Cqrs.ExecuteAsync(command);
}
