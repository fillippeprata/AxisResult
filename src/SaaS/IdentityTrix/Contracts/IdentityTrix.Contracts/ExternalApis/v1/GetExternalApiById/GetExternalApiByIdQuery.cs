using AxisTrix.CQRS.Queries;

namespace IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;

public record GetExternalApiByIdQuery: IAxisQuery<GetExternalApiByIdResponse>
{
    public string? ExternalApiId { get; init; }
}
