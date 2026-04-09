using AxisTrix.CQRS.Commands;

namespace IndentityTrix.Contracts.ExternalApis.v1.GenerateNewSecret;

public record GenerateNewExternalApiSecretResponse : IAxisCommandResponse
{
    public required string ExternalApiId { get; init; }
    public required string Secret { get; init; }
}
