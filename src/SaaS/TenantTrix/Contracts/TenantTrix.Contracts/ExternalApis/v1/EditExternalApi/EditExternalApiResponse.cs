using AxisMediator.Contracts.CQRS.Commands;

namespace TenantTrix.Contracts.ExternalApis.v1.EditExternalApi;

public record EditExternalApiResponse : IAxisCommandResponse
{
    public required string ExternalApiId { get; init; }
    public required string ApiName { get; init; }
}
