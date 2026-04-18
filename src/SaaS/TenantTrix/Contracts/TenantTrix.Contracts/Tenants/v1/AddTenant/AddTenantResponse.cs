using AxisMediator.Contracts.CQRS.Commands;

namespace TenantTrix.Contracts.Tenants.v1.AddTenant;

public record AddTenantResponse : IAxisCommandResponse
{
    public required string TenantId { get; init; }
    public required string TenantName { get; init; }
}
