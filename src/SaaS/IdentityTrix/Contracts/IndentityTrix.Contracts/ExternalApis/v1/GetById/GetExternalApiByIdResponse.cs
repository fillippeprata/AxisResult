using AxisTrix.CQRS.Queries;

namespace IndentityTrix.Contracts.ExternalApis.v1.GetById;

public record GetExternalApiByIdResponse : IAxisQueryResponse
{
    public required string ExternalApiId { get; init; }
    public required string Name { get; init; }
}
