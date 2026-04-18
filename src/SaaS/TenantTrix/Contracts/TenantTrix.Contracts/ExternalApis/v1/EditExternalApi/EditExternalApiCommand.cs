using AxisMediator.Contracts.CQRS.Commands;

namespace TenantTrix.Contracts.ExternalApis.v1.EditExternalApi;

public record EditExternalApiCommand : IAxisCommand<EditExternalApiResponse>
{
    public string? ExternalApiId { get; init; }
    public string? ApiName { get; init; }
}
