using AxisMediator.Contracts.CQRS.Queries;

namespace TenantTrix.Contracts.ExternalApis.v1.GetExternalApiById;

public record GetExternalApiByIdQuery: IAxisQuery<GetExternalApiByIdResponse>
{
    public string? ExternalApiId { get; init; }
}
