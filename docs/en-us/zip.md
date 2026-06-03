# Combine · `Zip`

> Joins **different values** into a tuple, to use them together later. Each `Zip` adds a value; if any of them fails, the whole tuple short-circuits.

---

## When to use

You need 2 to 4 values from distinct operations to build a result (a dashboard, a read aggregate).

## When *not* to use

| You want to… | Use instead |
|---|---|
| reduce **N** results of the **same** type into one | [`Combine`/`All`](aggregate.md) |
| replace the value (not accumulate) | [`Then`](then.md) |

---

## Operators

| Method | Semantics |
|---|---|
| `Zip` / `ZipAsync` | **sequential**, *fail-fast*; builds `(T1, T2)` … up to `(T1, T2, T3, T4)` |
| `ZipParallel` / `ZipParallelAsync` | runs both sides **concurrently**; if both fail, **accumulates** the errors |
| `Map((a, b) => …)` | destructures the tuple at the end — no `.Value1` / `.Value2` |

---

## Example 1 — build a dashboard (sequential)

```csharp
var dashboard = await GetUserAsync(userId)
    .ZipAsync(user    => GetAccountAsync(user.AccountId))   // (User, Account)
    .ZipAsync((u, ac) => GetPlanAsync(ac.PlanId))           // (User, Account, Plan)
    .MapAsync((user, account, plan) => new DashboardResponse
    {
        UserName       = user.Name,
        AccountBalance = account.Balance,
        PlanName       = plan.Name
    });
```

**Why it pays off:** the three values arrive together at the final `MapAsync`, destructured by name — and any step that fails interrupts the chain, with no nesting.

## Example 2 — independent operations (parallel)

When the two sides **don't depend** on each other, run them concurrently:

```csharp
var dashboard = await GetUserAsync(userId)
    .ZipParallelAsync(() => GetRecentOrdersAsync(userId))   // runs in parallel
    .MapAsync((user, orders) => new DashboardResponse
    {
        UserName   = user.Name,
        OrderCount = orders.Count
    });
```

**Why it pays off:** `ZipParallel` uses `Task.WhenAll` under the hood — the two *fetches* happen at the same time; and if **both** fail, you see **all** the errors at once, not just the first.

---

## See also

- [Transform · `Map`](map.md) — destructure the tuple with `(a, b) => …`
- [Aggregate · `Combine`/`All`](aggregate.md) — for N results of the same type
- [`Task` vs `ValueTask`](async-task-vs-valuetask.md) — choosing the async form in `ZipParallel`

---

↩ [Back to AxisResult docs](../../README.md)
