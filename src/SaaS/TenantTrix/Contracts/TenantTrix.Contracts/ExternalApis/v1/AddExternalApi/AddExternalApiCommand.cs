using AxisMediator.Contracts.CQRS.Commands;

namespace TenantTrix.Contracts.ExternalApis.v1.AddExternalApi;

public record AddExternalApiCommand : IAxisCommand<AddExternalApiResponse>
{
    public string? ApiName { get; init; }
    public string? TenantId { get; init; }
}
