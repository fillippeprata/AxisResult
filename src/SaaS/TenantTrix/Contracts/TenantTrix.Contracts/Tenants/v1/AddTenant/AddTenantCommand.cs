using AxisMediator.Contracts.CQRS.Commands;

namespace TenantTrix.Contracts.Tenants.v1.AddTenant;

public record AddTenantCommand : IAxisCommand<AddTenantResponse>
{
    public string? TenantName { get; init; }
}
