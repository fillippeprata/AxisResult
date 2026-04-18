using AxisMediator.Contracts.CQRS.Commands;

namespace TenantTrix.Contracts.Tenants.v1.EditTenant;

public record EditTenantCommand : IAxisCommand<EditTenantResponse>
{
    public string? TenantId { get; init; }
    public string? TenantName { get; init; }
}
