using Axis;
using IdentityTrix.Ports;

namespace IdentityTrix.Driven.Repositories.Postgres.UnitOfWork;

internal class UnitOfWorkProvider(IAxisUnitOfWork unitOfWork) : IUnitOfWorkProvider
{
    public IAxisUnitOfWork UnitOfWork { get; } = unitOfWork;
}
