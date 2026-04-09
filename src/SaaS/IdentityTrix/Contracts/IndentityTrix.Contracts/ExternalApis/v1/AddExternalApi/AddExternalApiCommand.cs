using AxisTrix.CQRS.Commands;

namespace IndentityTrix.Contracts.ExternalApis.v1.AddExternalApi;

public record AddExternalApiCommand : IAxisCommand<AddExternalApiResponse>
{
    public string? ApiName { get; init; }
}
