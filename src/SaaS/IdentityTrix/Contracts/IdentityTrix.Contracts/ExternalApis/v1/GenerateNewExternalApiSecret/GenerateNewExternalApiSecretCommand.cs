using AxisMediator.Contracts.CQRS.Commands;

namespace IdentityTrix.Contracts.ExternalApis.v1.GenerateNewExternalApiSecret;

public record GenerateNewExternalApiSecretCommand : IAxisCommand<GenerateNewExternalApiSecretResponse>
{
    public required string ExternalApiId { get; init; }
}
