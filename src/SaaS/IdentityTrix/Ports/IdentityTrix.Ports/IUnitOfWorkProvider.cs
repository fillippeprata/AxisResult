using Axis;

namespace IdentityTrix.Ports;

public interface IUnitOfWorkProvider
{
    IAxisUnitOfWork UnitOfWork { get; }
}
