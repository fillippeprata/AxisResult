using AxisMediator.Contracts.CQRS.Queries;

namespace IdentityTrix.Contracts.ExternalApis.v1.GetExternalApiById;

public record GetExternalApiByIdResponse : IAxisQueryResponse
{
    public required string ExternalApiId { get; init; }
    public required string Name { get; init; }
}
