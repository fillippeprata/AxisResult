using AxisMediator.Contracts.CQRS.Commands;

namespace IdentityTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;

public record GenerateNewExternalApiSecretResponse : IAxisCommandResponse
{
    public required string ExternalApiId { get; init; }
    public required string Secret { get; init; }
}
