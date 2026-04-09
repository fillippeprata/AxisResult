using AxisTrix.Repositories;
using Npgsql;

namespace AxisTrix.Repository.Postgres;

public interface IPostgresUnitOfWork : IAxisUnitOfWork
{
    Task<NpgsqlCommand> NewCommandAsync(string sql);
}
