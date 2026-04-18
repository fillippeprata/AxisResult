using Axis;

namespace TenantTrix.Ports;

public interface IUnitOfWorkProvider
{
    IAxisUnitOfWork UnitOfWork { get; }
}
