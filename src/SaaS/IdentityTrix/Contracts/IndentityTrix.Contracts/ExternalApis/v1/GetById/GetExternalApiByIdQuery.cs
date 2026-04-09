using AxisTrix.CQRS.Queries;

namespace IndentityTrix.Contracts.ExternalApis.v1.GetById;

public record GetExternalApiByIdQuery: IAxisQuery<GetExternalApiByIdResponse>
{
    public string? ExternalApiId { get; init; }
}
