using AxisTrix.Logging;
using Npgsql;

namespace AxisTrix.Repository.Postgres;

public abstract class PostgresRepositoryBase(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    IPostgresUnitOfWork uow)
{
    protected CancellationToken CancellationToken => mediator.CancellationToken;

    protected async Task<AxisResult> ExecuteAsync(string sql, Action<NpgsqlParameterCollection> addParams, string? duplicateKeyCode = "")
    {
        try
        {
            await using var command = await uow.NewCommandAsync(sql);
            addParams(command.Parameters);
            await command.ExecuteNonQueryAsync(CancellationToken);
            return await AxisResult.OkAsync();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("duplicate key value violates unique constraint"))
                return AxisError.Conflict(duplicateKeyCode ?? "POSTGRES_DUPLICATE_KEY_ERROR");

            logger.LogError(ex, "POSTGRES_EXECUTION_ERROR");
            return AxisError.InternalServerError("POSTGRES_EXECUTION_ERROR");
        }
    }

    protected async Task<AxisResult<T>> GetAsync<T>(
        string sql,
        Action<NpgsqlParameterCollection> addParams,
        Func<NpgsqlDataReader, T> map,
        string notFoundCode)
    {
        try
        {
            await using var command = await uow.NewCommandAsync(sql);
            addParams(command.Parameters);
            await using var reader = await command.ExecuteReaderAsync(CancellationToken);
            if (!await reader.ReadAsync(CancellationToken))
                return AxisError.NotFound(notFoundCode);
            return AxisResult.Ok(map(reader));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "POSTGRES_GET_ERROR");
            return AxisError.InternalServerError("POSTGRES_GET_ERROR");
        }
    }

    protected async Task<AxisResult<IEnumerable<T>>> ListAsync<T>(
        string sql,
        Action<NpgsqlParameterCollection> addParams,
        Func<NpgsqlDataReader, T> map)
    {
        try
        {
            await using var command = await uow.NewCommandAsync(sql);
            addParams(command.Parameters);
            await using var reader = await command.ExecuteReaderAsync(CancellationToken);
            var list = new List<T>();
            while (await reader.ReadAsync(CancellationToken))
                list.Add(map(reader));
            return AxisResult.Ok<IEnumerable<T>>(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "POSTGRES_LIST_ERROR");
            return AxisError.InternalServerError("POSTGRES_LIST_ERROR");
        }
    }
}
