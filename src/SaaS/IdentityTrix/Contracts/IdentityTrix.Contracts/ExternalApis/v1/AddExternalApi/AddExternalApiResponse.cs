using AxisMediator.Contracts.CQRS.Commands;

namespace IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;

public record AddExternalApiResponse : IAxisCommandResponse
{
    public required string ExternalApiId { get; init; }
    public required string Name { get; init; }
    public required string Secret { get; init; }
    public required string TenantId { get; init; }
}
