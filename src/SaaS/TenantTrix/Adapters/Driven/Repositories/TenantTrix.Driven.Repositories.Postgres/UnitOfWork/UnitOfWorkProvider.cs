using Axis;
using TenantTrix.Ports;

namespace TenantTrix.Driven.Repositories.Postgres.UnitOfWork;

internal class UnitOfWorkProvider(IAxisUnitOfWork unitOfWork) : IUnitOfWorkProvider
{
    public IAxisUnitOfWork UnitOfWork { get; } = unitOfWork;
}
