using System.Collections.Concurrent;
using AxisTrix.Logging;
using AxisTrix.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AxisTrix.Repository.Postgres;

public class PostgresUnitOfWorkProvider
{
    private readonly ConcurrentDictionary<object, PostgresUnitOfWork> _unitOfWorks = new();

    public PostgresUnitOfWork GetUnitOfWork(IServiceProvider sp, object? key)
    {
        if (_unitOfWorks.TryGetValue(key!, out var wow))
            return wow;

        var dataSource = sp.GetRequiredKeyedService<NpgsqlDataSource>(key);
        var mediator = sp.GetRequiredService<IAxisMediator>();
        var telemetry = sp.GetRequiredService<IAxisTelemetry>();
        var logger = sp.GetRequiredService<IAxisLogger<PostgresUnitOfWork>>();
        var newUow = new PostgresUnitOfWork(mediator, dataSource, telemetry, logger);
        _unitOfWorks.TryAdd(key!, newUow);
        return newUow;
    }
}
