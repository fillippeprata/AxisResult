using Axis;
using Npgsql;

namespace AxisRepository.Postgres;

public interface IPostgresUnitOfWork : IAxisUnitOfWork
{
    Task<NpgsqlCommand> NewCommandAsync(string sql);
}
