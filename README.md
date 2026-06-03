# AxisResult — Documentation

> 🌐 [Português (documentação navegável)](docs/pt-br/README.md)

**Railway-Oriented Programming for C#** — a zero-dependency *Result monad* with full `async`/`ValueTask` support, typed error categories and monadic composition (`Then` / `Map` / `Zip`).

```csharp
public Task<AxisResult<AddCellphoneResponse>> HandleAsync(AddCellphoneCommand cmd)
    => personFactory.GetByIdAsync(cmd.PersonId)
        .ThenAsync(person => cellphoneMediator.AddAsync(new() { CountryId = cmd.CountryId, Number = cmd.Number }))
        .ThenAsync(response => response.AddCellphoneAsync(cmd.CellphoneId))
        .ThenAsync(_ => unitOfWork.SaveChangesAsync())
        .MapAsync(_ => new AddCellphoneResponse { CellphoneId = cmd.CellphoneId });
```

Use this page as a **map**: read the trunk below (~5 min) and jump straight to the detail of the group you need — without reading hundreds of lines.

---

## The trunk (read first)

### Railway in 60 seconds

Imagine your code as a railway with two rails:

```
Success ━━━━━●━━━━━━━━━━●━━━━━━━━━━●━━━━▶  result
             │          │          │
          validate     fetch      save
             │          │          │
Failure ━━━━━╋━━━━━━━━━━╋━━━━━━━━━━╋━━━━▶  errors
```

Each operation **either** succeeds and stays on the top rail, **or** fails and drops to the bottom one — skipping everything else. No `try/catch`, no `if (x == null)`, no mid-handler `return`. → **[Railway-Oriented Programming](docs/en-us/railway-oriented-programming.md)**

### `AxisResult` vs `AxisResult<T>` — "no data" and "with data"

- **`AxisResult`** — the outcome of an operation that **produces no value**: only whether it worked matters (save, delete, validate, verify a password).
- **`AxisResult<T>`** — carries a **value** along the success rail (fetch an entity, compute a total). `.Value` throws on a failure → prefer the [safe deconstruction or `Match`](docs/en-us/match.md).
- Moving between the two: [`ToAxisResult`](docs/en-us/then.md) discards the value; [`WithValue`](docs/en-us/ensure.md) promotes an `AxisResult` to `AxisResult<T>`.

### Creating results

```csharp
AxisResult         ok    = AxisResult.Ok();
AxisResult<int>    typed = AxisResult.Ok(42);
AxisResult<int>    fail  = AxisError.BusinessRule("INSUFFICIENT_STOCK"); // AxisError → failure (implicit)
AxisResult<string> name  = "John";                                       // value → Ok (implicit)
AxisResult<int>    parse = AxisResult.Try(() => int.Parse(input));        // exception → AxisResult, only at the edge
```

### Error handling

An error is a **value** (`AxisError` = `Code` + `Type`), not an exception. The 12 categories map to HTTP status codes, and `IsTransient` enables retry. → **[Errors and types](docs/en-us/errors-and-types.md)**

### `Task` vs `ValueTask`

When in doubt, use `Task`. `ValueTask` only on *hot paths* that complete synchronously. → **[Task vs ValueTask](docs/en-us/async-task-vs-valuetask.md)**

### Installation

```
dotnet add package AxisResult
```

→ Full guide: **[Getting started](docs/en-us/getting-started.md)**

---

## The map (jump to what you need)

| Group                            | You want to…                                       | Detail                                  |
|----------------------------------|----------------------------------------------------|-----------------------------------------|
| **Transform · `Map`**            | change the value (cannot fail)                     | [map.md](docs/en-us/map.md)             |
| **Chain · `Then`** ⭐             | a step that **can fail** (heart of the library)    | [then.md](docs/en-us/then.md)           |
| **Ensure · `Ensure`**            | validate an invariant inline                       | [ensure.md](docs/en-us/ensure.md)       |
| **Exit · `Match`**               | collapse the pipeline into a final value           | [match.md](docs/en-us/match.md)         |
| **Side effects · `Tap`**         | observe (log/metric) without changing the rail     | [tap.md](docs/en-us/tap.md)             |
| **Recover · `Recover`**          | handle the failure and return to success           | [recover.md](docs/en-us/recover.md)     |
| **Combine · `Zip`**              | join **different** values into a tuple             | [zip.md](docs/en-us/zip.md)             |
| **Aggregate · `Combine`/`All`**  | reduce **N** results into one                      | [aggregate.md](docs/en-us/aggregate.md) |
| **Remap errors · `MapError`**    | rewrite errors between layers                      | [map-errors.md](docs/en-us/map-errors.md) |
| **Cancellation**                 | thread `CancellationToken` through the chain       | [cancellation.md](docs/en-us/cancellation.md) |

**Start here:** [Getting started](docs/en-us/getting-started.md) · [Railway-Oriented Programming](docs/en-us/railway-oriented-programming.md) · [Why AxisResult?](docs/en-us/why-axisresult.md)

**Fundamentals:** [Errors and types](docs/en-us/errors-and-types.md) · [`Task` vs `ValueTask`](docs/en-us/async-task-vs-valuetask.md) · [Exceptions at the boundary](docs/en-us/boundary-and-try.md)

**Reference & extras:** [API reference](docs/en-us/api-reference.md) · [LINQ query syntax](docs/en-us/linq-query-syntax.md) · [Ergonomics](docs/en-us/ergonomics.md)

---

## Design principles

1. **Errors are values, not exceptions.** An operation that can fail says so in its return type.
2. **The type system is the documentation.** `Task<AxisResult<User>>` already tells you everything that can happen.
3. **Composition over ceremony.** Small, focused operations that compose.
4. **Fail fast, recover deliberately.** Errors propagate on their own; recovery is always explicit.
5. **Exceptions at the boundary, results everywhere else.** `AxisResult.Try()` at infrastructure edges; above that, exception-free.

---

## License

Apache 2.0
