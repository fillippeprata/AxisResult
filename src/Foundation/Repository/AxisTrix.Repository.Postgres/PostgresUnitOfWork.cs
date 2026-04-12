using AxisResult;
using AxisTrix.Logging;
using AxisTrix.Telemetry;
using Npgsql;

namespace AxisTrix.Repository.Postgres;

public class PostgresUnitOfWork(
    IAxisMediator mediator,
    NpgsqlDataSource dataSource,
    IAxisTelemetry telemetry,
    IAxisLogger<PostgresUnitOfWork> logger
) : IPostgresUnitOfWork
{
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public async Task<NpgsqlCommand> NewCommandAsync(string sql)
    {
        if (_connection == null || _transaction == null)
            await StartAsync();

        return new(sql, _connection, _transaction);
    }

    public async Task<AxisResult.AxisResult> StartAsync()
    {
        var ct = mediator.CancellationToken;
        using var span = telemetry.StartSpan("db.postgres.connect", AxisSpanKind.Client);
        span.SetTag("db.system", "postgresql");

        try
        {
            _connection ??= await dataSource.OpenConnectionAsync(ct);
            _transaction = await _connection.BeginTransactionAsync(ct);
            span.SetStatus(AxisSpanStatus.Ok);
            return AxisResult.AxisResult.Ok();
        }
        catch (Exception ex)
        {
            span.RecordException(ex);
            logger.LogError(ex, "Failed to open Postgres connection");
            return AxisError.InternalServerError("POSTGRES_ERROR_STARTING_CONNECTION");
        }
    }

    public async Task<AxisResult.AxisResult> SaveChangesAsync()
    {
        if (_transaction == null)
            return AxisError.InternalServerError("POSTGRES_TRANSACTION_NOT_STARTED");

        using var span = telemetry.StartSpan("db.postgres.commit", AxisSpanKind.Client);
        span.SetTag("db.system", "postgresql");

        try
        {
            await _transaction.CommitAsync(mediator.CancellationToken);
            _transaction = null;
            span.SetStatus(AxisSpanStatus.Ok);
            return AxisResult.AxisResult.Ok();
        }
        catch (Exception ex)
        {
            span.RecordException(ex);
            logger.LogError(ex, "Failed to commit Postgres transaction");
            return AxisError.InternalServerError("POSTGRES_SAVING_CHANGES_ERROR");
        }
    }

    public async Task<AxisResult.AxisResult> RollbackAsync()
    {
        using var span = telemetry.StartSpan("db.postgres.rollback", AxisSpanKind.Client);
        span.SetTag("db.system", "postgresql");

        try
        {
            if (_transaction != null) await _transaction.RollbackAsync(mediator.CancellationToken);
            span.SetStatus(AxisSpanStatus.Ok);
            return AxisResult.AxisResult.Ok();
        }
        catch (Exception ex)
        {
            span.RecordException(ex);
            logger.LogError(ex, "Failed to rollback Postgres transaction");
            return AxisError.InternalServerError("POSTGRES_ROLLBACK_ERROR");
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await (_connection?.DisposeAsync() ?? ValueTask.CompletedTask);
        GC.SuppressFinalize(this);
    }
}
