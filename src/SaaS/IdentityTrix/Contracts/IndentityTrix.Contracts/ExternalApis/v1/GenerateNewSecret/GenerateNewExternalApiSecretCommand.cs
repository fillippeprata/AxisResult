using AxisTrix.CQRS.Commands;

namespace IndentityTrix.Contracts.ExternalApis.v1.GenerateNewSecret;

public record GenerateNewExternalApiSecretCommand : IAxisCommand<GenerateNewExternalApiSecretResponse>
{
    public required string ExternalApiId { get; init; }
}
