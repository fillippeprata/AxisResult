using AxisTrix.CQRS.Commands;

namespace IndentityTrix.Contracts.Authentication.v1.AuthenticateExternalApi;

public record AuthenticateExternalApiCommand : IAxisCommand
{
    public required string ExternalApiId { get; init; }
    public required string Secret { get; init; }
}
