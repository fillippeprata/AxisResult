using AxisTrix.CQRS.Commands;

namespace IdentityTrix.Contracts.ExternalApis.v1.AddExternalApi;

public record AddExternalApiCommand : IAxisCommand<AddExternalApiResponse>
{
    public string? ApiName { get; init; }
}
