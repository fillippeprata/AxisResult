using Axis;
using DataPrivacyTrix.Ports;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.UnitOfWork;

internal class UnitOfWorkProvider(IAxisUnitOfWork unitOfWork) : IUnitOfWorkProvider
{
    public IAxisUnitOfWork UnitOfWork { get; } = unitOfWork;
}
