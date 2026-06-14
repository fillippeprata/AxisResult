# Ensure · `Ensure`

> Validates an invariant **inside** the pipeline. If the guard fails, the rail switches to failure and the rest is skipped.

---

## When to use

Guarantee a condition about the current value (enough stock, valid status) or enforce "only continue if it doesn't already exist".

## When *not* to use

| You want to…                               | Use instead             |
|--------------------------------------------|-------------------------|
| transform the value                        | [`Map`](map.md)         |
| chain a step that produces a new value     | [`Then`](then.md)       |
| automatic validation **before** the handler | (validation pipeline)   |

---

## Operators

| Method | Signature | What it does |
|---|---|---|
| `Ensure` | `(Func<T,bool> predicate, AxisError error)` | fails with `error` if the predicate is false |
| `Ensure` | `(Func<T,AxisResult> validation)` | delegated validation that returns `AxisResult` |
| `RequireNotFound` | `(AxisError errorIfFound)` | found → fail; `NotFound` → continue as success |
| `WithValueAsync` | `(value)` | promotes an `AxisResult` (no value) to `AxisResult<T>` (async only — `Task`/`ValueTask`) |

All have `Async` variants (`Task`/`ValueTask`) and [with `CancellationToken`](cancellation.md).

---

## Example 1 — business-rule guard

```csharp
return GetProductAsync(cmd.ProductId) // AxisResult<Product>
    .EnsureAsync(p => p.Stock >= cmd.Quantity, AxisError.BusinessRule("INSUFFICIENT_STOCK"))
    .ThenAsync(p => reserveStockPort.ReserveAsync(p.Id, cmd.Quantity));
```

**Why it pays off:** the "is there stock?" rule stays **on the rail itself**, as a readable step, instead of a loose `if` with a `return BadRequest` in the middle of the handler.

## Example 2 — create only if it doesn't exist (idempotency)

`RequireNotFound` turns "not found" into success, and any other outcome into failure:

```csharp
public Task<AxisResult<IPersonAggregateApplication>> CreateAsync(NewArgs args)
    => readerPort.GetByNationalIdAsync(args.NationalId)                          // search
        .RequireNotFoundAsync(AxisError.Conflict("DOCUMENT_ALREADY_EXISTS"))     // found → fail
        .WithValueAsync(new PersonEntity(args.NationalId, args.DisplayName))     // not found → create
        .MapAsync(NewInstance);
```

**Why it pays off:** the "create if it doesn't exist" pattern — which normally requires an `if (found) throw` — becomes three declarative steps that read like the rule speaks.

---

## See also

- [Chain · `Then`](then.md) — the step that follows the guard
- [Errors and types](errors-and-types.md) — choosing the right `AxisError` for the failure
- [Recover · `Recover`](recover.md) — the opposite: handle the failure and return to success

---

↩ [Back to AxisResult docs](../../README.md)
