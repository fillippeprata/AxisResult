# Aggregate · `Combine` / `All`

> Reduces **many** results into **one**. Unlike [`Zip`](zip.md) (which combines different values into a tuple), here you fold a **collection** — and collect **all** the errors, not just the first.

---

## When to use

Validate several fields at once (wanting to see all the failures), or consolidate a list of operations of the same type into a single result.

---

## Operators

| Method | In | Out |
|---|---|---|
| `Combine(params results)` | N × `AxisResult` (no value) | `AxisResult` — **all** the errors together |
| `All(results)` | N × `AxisResult<T>` | `AxisResult<IReadOnlyList<T>>` |
| `CombineAsync` / `AllAsync` | versions for `Task`/`ValueTask` | same |

---

## Example 1 — validate everything and show all failures

```csharp
var result = AxisResult.Combine(
    ValidateName(cmd.Name),
    ValidateEmail(cmd.Email),
    ValidateAge(cmd.Age));
// collects ALL the errors, not just the first
```

**Why it pays off:** the user sees "empty name **and** invalid email" at once, instead of fixing one, resubmitting, and only then discovering the next. A single validation *round-trip*.

## Example 2 — consolidate a list of the same type

```csharp
var result = await AxisResult.AllAsync(
    userIds.Select(id => GetUserAsync(id)));
// AxisResult<IReadOnlyList<User>> — either all the users, or all the errors
```

**Why it pays off:** "fetch N and continue only if all came back" becomes one line; if any fails, the aggregated errors bubble up together.

---

## `Combine`/`All` vs `Zip`

- **`Combine`/`All`** → N items of the **same** type → a list (or an aggregated void).
- **[`Zip`](zip.md)** → 2–4 **different** values → a tuple.

---

## See also

- [Combine · `Zip`](zip.md) — for heterogeneous values in a tuple
- [Errors and types](errors-and-types.md) — why accumulating all the errors matters
- [Ensure · `Ensure`](ensure.md) — validation of a single value on the rail

---

↩ [Back to AxisResult docs](../../README.md)
