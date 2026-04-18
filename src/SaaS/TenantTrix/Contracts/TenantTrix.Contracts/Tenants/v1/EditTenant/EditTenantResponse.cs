using AxisMediator.Contracts.CQRS.Commands;

namespace TenantTrix.Contracts.Tenants.v1.EditTenant;

public record EditTenantResponse : IAxisCommandResponse
{
    public required string TenantId { get; init; }
    public required string TenantName { get; init; }
}
