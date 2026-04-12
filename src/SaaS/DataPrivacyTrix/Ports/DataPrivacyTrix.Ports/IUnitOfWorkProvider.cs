using AxisTrix.Repositories;

namespace DataPrivacyTrix.Ports;

public interface IUnitOfWorkProvider
{
    IAxisUnitOfWork UnitOfWork { get; }
}
