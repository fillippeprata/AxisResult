namespace AxisTrix.Repositories;

public interface IAxisUnitOfWork : IDisposable, IAsyncDisposable
{
    Task<AxisResult> StartAsync();
    Task<AxisResult> SaveChangesAsync();
    Task<AxisResult> RollbackAsync();

    /// <summary>
    /// Executes <paramref name="work"/> inside a transaction, commits on success and
    /// rolls back on failure or exception. Exceptions are re-thrown after rollback.
    /// </summary>
    async Task<AxisResult> InTransactionAsync(Func<Task<AxisResult>> work)
    {
        var start = await StartAsync();
        if (start.IsFailure) return start;

        try
        {
            var result = await work();
            if (result.IsFailure)
            {
                await RollbackAsync();
                return result;
            }
            return await SaveChangesAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Executes <paramref name="work"/> inside a transaction, commits on success and
    /// rolls back on failure or exception. Exceptions are re-thrown after rollback.
    /// </summary>
    async Task<AxisResult<T>> InTransactionAsync<T>(Func<Task<AxisResult<T>>> work)
    {
        var start = await StartAsync();
        if (start.IsFailure) return start.Errors.ToArray();

        try
        {
            var result = await work();
            if (result.IsFailure)
            {
                await RollbackAsync();
                return result;
            }
            var save = await SaveChangesAsync();
            if (save.IsFailure) return save.Errors.ToArray();
            return result;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }
}
