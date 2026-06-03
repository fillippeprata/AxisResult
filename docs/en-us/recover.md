# Recover · `Recover`

> The opposite of [`Then`](then.md): it operates on the **failure rail**. `Recover` and its relatives bring the pipeline back to success — always in a **deliberate and explicit** way.

---

## When to use

Provide a default when something wasn't found, fall back to an alternative source when a service is down, try a second path.

## When *not* to use

| You want to… | Use instead |
|---|---|
| just **observe** the error (log) without recovering | [`TapError`](tap.md) |
| **rewrite** the error, not recover | [`MapError`](map-errors.md) |

---

## Operators

| Method | Recovers when… |
|---|---|
| `Recover(value)` / `Recover(func)` | **any** failure → default value |
| `RecoverWhen(AxisErrorType, func)` | the errors are of a **type** |
| `RecoverWhen(code, func)` | the errors have a **code** |
| `RecoverWhen(predicate, func)` | a **predicate** over the errors |
| `RecoverNotFound(func)` | **all** errors are `NotFound` |
| `OrElse(fallback)` | tries an alternative **operation** |
| `OrElse(fallback, combineErrors: true)` | alternative, **accumulating** the errors from both sides |

All have `Async` variants (`Task`/`ValueTask`).

---

## Example 1 — default when it doesn't exist

```csharp
var settings = await GetUserSettingsAsync(userId)
    .RecoverNotFoundAsync(() => DefaultSettings.Create());
```

**Why it pays off:** "no settings saved? use the default" stops being a `catch (NotFoundException)` and becomes an explicit intent — and it only recovers from `NotFound`, not from real errors (e.g. a database failure).

## Example 2 — conditional fallback by type

```csharp
var data = await FetchFromPrimaryAsync()
    .RecoverWhenAsync(AxisErrorType.ServiceUnavailable, () => FetchFromFallbackAsync());
```

**Why it pays off:** it only falls back to the secondary when the primary is **unavailable** (transient); a validation error, for instance, keeps failing as it should.

## Example 3 — alternative path with error accumulation

```csharp
var user = await FindByEmailAsync(email)
    .OrElseAsync(_ => FindByPhoneAsync(phone), combineErrors: true);
// if BOTH fail, you get the errors from both attempts
```

---

## See also

- [Errors and types](errors-and-types.md) — `IsTransient`, types and codes to condition the recovery
- [Remap errors · `MapError`](map-errors.md) — transform the error instead of recovering
- [Ensure · `Ensure`](ensure.md) — the opposite: take from success to failure

---

↩ [Back to AxisResult docs](../../README.md)
