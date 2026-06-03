# Exceptions at the boundary · `Try` and adapters

> "Exceptions at the boundary, results everywhere else." This page shows how to convert exception-throwing infrastructure surfaces (database, HTTP, brokers) into `AxisResult` at a single point — and what `Try` does **not** catch.

---

## When to use

At any point where external code throws exceptions: database drivers, `HttpClient`, message brokers, file I/O. Above that boundary, everything is exception-free.

## When *not* to use

| You want to… | Use instead |
|---|---|
| chain steps that already return `AxisResult` | [`Then`](then.md) |
| recover from a transient failure | [`Recover`](recover.md) |

---

## Real-world example — database access without exceptions

```csharp
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
```

**Why it pays off:** the only `try/catch` in the entire architecture lives at the infrastructure boundary. Database exceptions are converted to `AxisResult` once, at the edge. Everything above — handlers, factories, domain rules — is exception-free.

---

## Boundary adapter pattern

For `HttpClient`, database drivers, message brokers and any other infrastructure with a leaky exception surface, the recommended pattern is to write a thin **adapter** whose job is *exclusively* converting the external surface into `AxisResult<Response>`. Every exception the underlying client can throw is mapped to a typed `AxisError` (timeouts → `Timeout`, 5xx → `ServiceUnavailable`, 401 → `Unauthorized`, etc.). The rest of the application consumes only `AxisResult<Response>` and never sees a raw `HttpResponseMessage`. Dedicated helper libraries for the HTTP and repository adapters are on the roadmap.

---

## Note on `Try` / `TryAsync`

`AxisResult.Try` does **not** catch "programmer error" exceptions — `NullReferenceException`, `ArgumentNullException`, `OperationCanceledException`, `OutOfMemoryException`, `StackOverflowException` and `ThreadAbortException` are rethrown. These represent bugs or genuinely unrecoverable situations and should not be silently turned into a result value. If you want a specific exception type captured, pass an `errorHandler` override or catch it manually in your adapter.

---

## See also

- [Chain · `Then`](then.md) — what consumes the `AxisResult` the boundary produces
- [Errors and types](errors-and-types.md) — the types to map each exception to
- [Remap errors · `MapError`](map-errors.md) — translate codes when crossing layers

---

↩ [Back to AxisResult docs](../../README.md)
