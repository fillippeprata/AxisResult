using Axis;

namespace DataPrivacyTrix.Ports;

public interface IUnitOfWorkProvider
{
    IAxisUnitOfWork UnitOfWork { get; }
}
