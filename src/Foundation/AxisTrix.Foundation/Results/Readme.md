# AxisResult — Railway Oriented Programming (ROP) for .NET

`AxisResult` is an implementation of the **Result** pattern (also known as **Either** in the functional community) focused on *Railway Oriented Programming*: pipelines of operations that may fail, chained in a way that failure short-circuits the rest of the track without exceptions.

---

## Quick Reference — Which method do I use?

### Creating Results

| Method | When to Use |
|--------|-------------|
| `AxisResult.Ok()` | Create a success result with no value |
| `AxisResult.Ok(value)` | Create a success result carrying a value |
| `AxisResult.Error(error)` | Create a failure result from one `AxisError` |
| `AxisResult.Error(errors)` | Create a failure result from multiple `AxisError`s |
| `AxisResult.OkAsync()` / `.OkAsync(value)` | Same as above, wrapped in `Task.FromResult` |
| `AxisResult.ErrorAsync(error)` | Same as `Error`, wrapped in `Task.FromResult` |
| `AxisResult.Try(func)` | Convert code that may throw into an `AxisResult` (catches non-critical exceptions) |
| `AxisResult.TryAsync(func)` | Async version of `Try` |
| `AxisError.NotFound("CODE")` | Create an error with a specific category (factories: `NotFound`, `ValidationRule`, `Conflict`, `BusinessRule`, `Unauthorized`, `Forbidden`, `InternalServerError`, `ServiceUnavailable`, `Timeout`, `TooManyRequests`, `GatewayTimeout`, `Mapping`) |
| Implicit: `AxisError` → `AxisResult` | Assign an `AxisError` directly where an `AxisResult` is expected |
| Implicit: `TValue` → `AxisResult<TValue>` | Assign a value directly where an `AxisResult<T>` is expected |

### Transforming & Chaining

| Method | When to Use |
|--------|-------------|
| `.Map(func)` | Transform the success value (pure, cannot fail): `User → string` |
| `.MapAsync(func)` | Async version of `Map` |
| `.Then(func)` | Chain to an operation that **may fail** (returns `AxisResult`): `User → AxisResult<Order>` |
| `.ThenAsync(func)` | Async version of `Then` |
| `.Ensure(predicate, error)` | Guard clause — fails with `error` if predicate is false |
| `.Ensure(func)` | Delegated validation — `func` returns `AxisResult`, fails if that result fails |
| `.EnsureAsync(predicate, error)` | Async version of `Ensure` |

### Combining Values (Zip)

| Method | When to Use |
|--------|-------------|
| `.Zip(func)` | Combine current value with a new one into a tuple `(T1, T2)` — pure mapper or failable (`AxisResult<TNew>`) |
| `.ZipAsync(func)` | Async version of `Zip` |
| `.Zip(...).Zip(...)` | Chain zips to build `(T1, T2, T3)` up to `(T1, T2, T3, T4)` |
| `.MapAsync((a, b) => ...)` | Transform a tuple after `Zip` — deconstructed params, no `.Value1`/`.Value2` needed |
| `.MapAsync((a, b, c) => ...)` | Same for 3-element tuples |
| `.MapAsync((a, b, c, d) => ...)` | Same for 4-element tuples |

### Side Effects

| Method | When to Use |
|--------|-------------|
| `.Tap(action)` | Execute side effect on success (logging, metrics), returns original result |
| `.Tap(action<TValue>)` | Same, with access to the value |
| `.TapAsync(func)` | Async side effect on success |
| `.TapError(action)` | Execute side effect on failure (logging errors) |
| `.TapErrorAsync(func)` | Async version of `TapError` |

### Aggregating Multiple Results

| Method | When to Use |
|--------|-------------|
| `AxisResult.Combine(r1, r2, r3)` | Join N untyped results — collects **all** errors (validation-style) |
| `AxisResult.CombineAsync(tasks)` | Async version of `Combine` (Task or ValueTask) |
| `AxisResult.All(results)` | Join N typed `AxisResult<T>` → `AxisResult<IReadOnlyList<T>>` — collects all errors |
| `AxisResult.AllAsync(tasks)` | Async version of `All` (Task or ValueTask) |

### Existence Guard

| Method | When to Use |
|--------|-------------|
| `.RequireNotFound(error)` | Invert a lookup: **success (found) → error**, **NotFound → Ok()**, other errors propagate. Use for "create if not exists" flows |
| `.RequireNotFoundAsync(error)` | Async extension (Task/ValueTask) version of `RequireNotFound` |

### Recovery & Fallback

| Method | When to Use |
|--------|-------------|
| `.Recover(value)` | On failure, replace with a fixed default value |
| `.Recover(func)` | On failure, compute a fallback value |
| `.Recover(func<errors>)` | On failure, compute fallback using the error list |
| `.RecoverAsync(func)` | Async version of `Recover` |
| `.RecoverWhen(predicate, func)` | Recover only if errors match a condition |
| `.RecoverWhen(AxisErrorType, func)` | Recover only for a specific error type |
| `.RecoverWhen("CODE", func)` | Recover only for a specific error code |
| `.RecoverNotFound(func)` | Shortcut — recover only if **all** errors are `NotFound` |
| `.OrElse(fallback)` | On failure, try an alternative operation that returns `AxisResult` |
| `.OrElse(fallback, combineErrors: true)` | Same, but accumulates errors from both paths if fallback also fails |
| `.OrElseAsync(fallback)` | Async version of `OrElse` |

### Error Transformation

| Method | When to Use |
|--------|-------------|
| `.MapError(func<AxisError, AxisError>)` | Transform each error individually (e.g., remap codes between layers) |
| `.MapError(func<errors, IEnumerable>)` | Transform/filter the entire error list |
| `.MapErrorAsync(func)` | Async version of `MapError` |

### Branching & Terminal

| Method | When to Use |
|--------|-------------|
| `.Match(onSuccess, onFailure)` | Convert result to a final type (e.g., `IActionResult`) — always runs exactly one branch |
| `.MatchAsync(onSuccess, onFailure)` | Async version of `Match` |

### LINQ Query Syntax

| Method | When to Use |
|--------|-------------|
| `from x in result select ...` | LINQ `Select` — same as `Map` |
| `from x in r1 from y in r2 select ...` | LINQ `SelectMany` — same as chained `Then`, short-circuits on first failure |
| `.SelectManyAsync(binder, projector)` | Async LINQ-style chaining |

### Async Conversion

| Method | When to Use |
|--------|-------------|
| `.AsTaskAsync()` | Wrap a sync `AxisResult` into `Task<AxisResult>` |
| `.AsValueTaskAsync()` | Wrap a sync `AxisResult` into `ValueTask<AxisResult>` |

> **Task vs ValueTask**: every combinator above exists in both `Task` and `ValueTask` variants. Use `ValueTask` on hot paths to avoid allocations; use `Task` when you need `Task.WhenAll` or similar APIs.

### AxisError Properties

| Property | Description |
|----------|-------------|
| `.Code` | Stable string identifier (e.g., `"USER_NOT_FOUND"`) |
| `.Type` | `AxisErrorType` enum category |
| `.IsTransient` | `true` for `ServiceUnavailable`, `Timeout`, `TooManyRequests`, `GatewayTimeout` — safe to retry |

---

## Table of Contents

1. [Concept](#concept)
2. [Motivation](#motivation)
3. [Data Model](#data-model)
4. [Creating Results](#creating-results)
5. [Functional Combinators](#functional-combinators)
6. [Async: Task and ValueTask](#async-task-and-valuetask)
7. [LINQ Query Syntax](#linq-query-syntax)
8. [Exception Handling (`Try`)](#exception-handling-try)
9. [Combination and Aggregation](#combination-and-aggregation)
10. [Recovery (`Recover`, `OrElse`)](#recovery-recover-orelse)
11. [Design Decisions](#design-decisions)
12. [End-to-End Examples](#end-to-end-examples)

---

## Concept

In ROP, every function that may fail returns a **Result** instead of throwing an exception. Combinators (`Map`, `Then`, `Ensure`, etc.) chain functions so that, at the first error, the rest of the pipeline is skipped (short-circuit) and the error "travels along the parallel track" until it is handled.

```
Input ─> Op1 ─> Op2 ─> Op3 ─> Output    (success track)
           \     \     \
            ─────────────> Errors        (failure track — short-circuit)
```

`AxisResult` provides two main types:

- **`AxisResult`** — a result with no value (success/failure).
- **`AxisResult<TValue>`** — a result **with a value** on success; **with a list of errors** on failure.

---

## Motivation

Well-known trade-offs of using exceptions for control flow:

- **Type invisibility**: the signature `User GetUser(int id)` does not indicate it may throw `NotFoundException`.
- **Stack unwinding cost** on hot paths.
- **try/catch noise** repeated at every layer that needs to handle expected cases.
- **Mixing domain errors with bugs**: `catch (Exception)` may swallow a `NullReferenceException`.

`AxisResult` treats **domain errors as values**: something a function *can* return, made explicit by its type. Exceptions still exist for what is truly exceptional (bugs, unavailable resources, cancellation).

---

## Data Model

### `AxisError`

Represents an individual error. **Minimalist by design** — carries only `Code` (a stable identifier) and `Type` (category). UI messages and metadata belong in upper layers.

```csharp
public record AxisError
{
    public string Code { get; init; }        // stable identifier: "USER_NOT_FOUND"
    public AxisErrorType Type { get; init; } // category: NotFound, ValidationRule, ...
    public bool IsTransient { get; }          // true for ServiceUnavailable/Timeout/etc.
}
```

**Named factories** for each category:

```csharp
AxisError.NotFound("USER_NOT_FOUND")
AxisError.ValidationRule("EMAIL_INVALID")
AxisError.Conflict("SKU_DUPLICATE")
AxisError.BusinessRule("CREDIT_LIMIT_EXCEEDED")
AxisError.Unauthorized("TOKEN_EXPIRED")
AxisError.Forbidden("ROLE_REQUIRED")
AxisError.InternalServerError("UNEXPECTED")
AxisError.ServiceUnavailable("DB_DOWN")
AxisError.Timeout("UPSTREAM_TIMEOUT")
AxisError.TooManyRequests("RATE_LIMIT")
AxisError.GatewayTimeout("GATEWAY")
AxisError.Mapping("DTO_MISMATCH")
```

### `AxisErrorType`

Enum of categories. `IsTransient` maps the categories that can be retried:
`ServiceUnavailable`, `Timeout`, `TooManyRequests`, `GatewayTimeout`.

### `AxisResult` / `AxisResult<TValue>`

`AxisResult` is `abstract` — the concrete implementation (`AxisResultImpl`) is `internal`. Consumers always use the abstract type via factories or implicit operators.

```csharp
public bool IsSuccess { get; }                    // true if error list is empty/null
public bool IsFailure { get; }                    // !IsSuccess
public IReadOnlyList<AxisError> Errors { get; }   // empty on success
public string JoinErrorCodes(string sep = ", ");  // utility for logging
```

`AxisResult<TValue>` adds:

```csharp
public TValue Value { get; }  // throws NoAccessValueOnErrorResultException if IsFailure
```

**Accessing `Value` on a failure result throws an exception.** This is intentional: if you reach the point of accessing `Value` without having checked `IsSuccess` (or without going through a combinator that guarantees the check), there is a bug in the flow.

---

## Creating Results

### Static Factories

```csharp
// Success
AxisResult ok = AxisResult.Ok();
AxisResult<User> okUser = AxisResult.Ok(user);

// Failure (implicit conversion)
AxisResult err = AxisError.NotFound("U1");
AxisResult<User> errUser = AxisError.NotFound("U1");

// Async versions (Task.FromResult wrappers)
Task<AxisResult> t = AxisResult.OkAsync();
Task<AxisResult<User>> tu = AxisResult.OkAsync(user);
Task<AxisResult> te = AxisResult.ErrorAsync(err);
```

Available operators:
- `AxisError` → `AxisResult` / `AxisResult<T>`
- `List<AxisError>` → `AxisResult` / `AxisResult<T>`
- `AxisError[]` → `AxisResult` / `AxisResult<T>`
- `TValue` → `AxisResult<TValue>`

---

## Functional Combinators

Each combinator has a defined semantics:

| Combinator | Success | Failure | Use |
|---|---|---|---|
| `Map(f)` | applies f to the value → new result | propagates error | pure transformation |
| `Then(f)` | calls f which returns another Result | propagates error | chains an operation that may fail |
| `Tap(act)` | executes side-effect, returns this | skips | logging/metrics on success |
| `TapError(act)` | skips | executes action with errors | logging/metrics on failure |
| `Ensure(pred, err)` | validates predicate, fails if false | propagates | inline validation |
| `Zip(f)` | combines current value with new one | propagates | joining two values |
| `Match(ok, err)` | calls `ok(value)` | calls `err(errors)` | convert to final type |
| `MapError(f)` | returns this | transforms errors | adapting codes between layers |
| `Recover(f)` | returns this | converts errors into a value | fallback |
| `RecoverWhen(cond, f)` | returns this | converts if condition matches | selective fallback |
| `OrElse(f)` | returns this | tries another result | alternative |

### `Map` — value transformation

```csharp
AxisResult<User> user = GetUser(id);
AxisResult<string> name = user.Map(u => u.Name);   // User → string
AxisResult<string> upper = user.Map(u => u.Name.ToUpper());
```

### `Then` — chaining an operation that may fail

```csharp
AxisResult<User> user = GetUser(id);
AxisResult<Order> order = user.Then(u => GetLastOrder(u.Id));
// GetLastOrder returns AxisResult<Order>, short-circuits if user fails
```

### `Tap` / `TapError` — side-effects

```csharp
result
    .Tap(u => logger.LogInformation("User {Id} loaded", u.Id))
    .TapError(errs => logger.LogWarning("Failed: {Codes}", errs.Select(e => e.Code)));
```

`Tap` is available in 4 forms on `AxisResult<TValue>`:
- `Tap(Action)` — side-effect without accessing the value
- `Tap(Action<TValue>)` — side-effect using the value
- `TapAsync(Func<Task>)` — async without value
- `TapAsync(Func<TValue, Task>)` — async using the value

### `Ensure` — inline validation

```csharp
var ageResult = AxisResult.Ok(age)
    .Ensure(a => a >= 18, AxisError.ValidationRule("MINOR"))
    .Ensure(a => a < 120, AxisError.ValidationRule("UNREALISTIC"));
```

Variant with another `AxisResult` (delegated validation):

```csharp
result.Ensure(user => ValidateUser(user));  // ValidateUser returns AxisResult
```

### `Zip` — combining two values

```csharp
AxisResult<User> user = GetUser(id);
AxisResult<(User, Profile)> withProfile = user.Zip(u => GetProfile(u.Id));
// GetProfile returns Profile or AxisResult<Profile>

// Ternary chain
AxisResult<(User, Profile, Settings)> tri = user
    .Zip(u => GetProfile(u.Id))           // (User, Profile)
    .Zip((u, p) => GetSettings(u.Id));    // (User, Profile, Settings)
```

`Zip` overloads:
- `Zip<TNew>(Func<TValue, TNew>)` — pure mapper
- `Zip<TNew>(Func<TValue, AxisResult<TNew>>)` — mapper that may fail

Extensions for `Task<AxisResult<(T1,T2)>>` / `ValueTask<AxisResult<(T1,T2)>>` (and up to `(T1,T2,T3)`) allow continuing the zip up to 4 elements (via named tuples). Each arity also exposes `MapAsync` with deconstructed parameters:

```csharp
// MapAsync on tuples — no need to access .Value1/.Value2 manually
await userTask
    .ZipAsync(u => GetProfile(u.Id))              // Task<AxisResult<(User, Profile)>>
    .MapAsync((user, profile) => new Dto(user, profile));  // Func<User, Profile, Dto>
```

### `Match` — converting to the final type

Common path for HTTP responses:

```csharp
IActionResult Response(AxisResult<User> r) => r.Match(
    onSuccess: user => Ok(user),
    onFailure: errs => errs[0].Type switch
    {
        AxisErrorType.NotFound       => NotFound(errs),
        AxisErrorType.ValidationRule => BadRequest(errs),
        AxisErrorType.Unauthorized   => Unauthorized(errs),
        _                            => StatusCode(500, errs)
    });
```

### `MapError` — adapting codes between layers

Useful at domain boundaries (e.g., mapping a persistence error to a domain error):

```csharp
result.MapError(err => err.Code == "DB_UNIQUE_VIOLATION"
    ? AxisError.Conflict("USER_ALREADY_EXISTS")
    : err);

// By list (allows filtering/grouping)
result.MapError(errs => errs.Where(e => !e.IsTransient));
```

---

## Async: Task and ValueTask

Every synchronous operation has an `Async` version. There are two separate worlds: `Task<AxisResult<T>>` and `ValueTask<AxisResult<T>>`. Each has its own set of methods and extensions — **they do not mix automatically**.

```csharp
// Task
Task<AxisResult<User>> userTask = LoadAsync(id);
Task<AxisResult<Order>> order = userTask
    .ThenAsync(u => FetchOrderAsync(u.Id))
    .EnsureAsync(o => o.Total > 0, AxisError.ValidationRule("EMPTY_ORDER"))
    .TapAsync(o => logger.LogInformation("Order {Id}", o.Id));
```

**Extensions over `Task<AxisResult<T>>`** live in `AxisResultExtensions`. They allow chaining without repeatedly writing `await`:

```csharp
var finalResult = await GetUserAsync(id)      // Task<AxisResult<User>>
    .ThenAsync(u => GetOrderAsync(u.Id))      // Task<AxisResult<Order>>
    .MapAsync(o => o.Total)                   // Task<AxisResult<decimal>>
    .EnsureAsync(t => t > 0, err);            // Task<AxisResult<decimal>>
```

**Converters between worlds**:

```csharp
Task<AxisResult> t = axisResult.AsTaskAsync();
ValueTask<AxisResult> vt = axisResult.AsValueTaskAsync();
```

### Why separate `Task` and `ValueTask`?

- **`Task<>`**: heap-allocated, but integrates with everything (`Task.WhenAll`, `ConfigureAwait`, etc.).
- **`ValueTask<>`**: zero allocation on synchronous/completed paths, but with strict rules (do not await twice, etc.).

Using `ValueTask` in hot handlers avoids ~40–80 bytes per call. In heavy I/O handlers, `Task` is generally more appropriate (`Task.WhenAll` exists; `ValueTask` would require a manual loop).

---

## LINQ Query Syntax

`AxisResult<T>` implements `Select`/`SelectMany`, enabling:

```csharp
AxisResult<decimal> totalWithDiscount =
    from user in GetUser(id)
    from order in GetLastOrder(user.Id)
    from discount in GetDiscount(user.Tier)
    select order.Total * (1 - discount);
```

Each `from` that returns `AxisResult<T>` becomes a `SelectMany` — short-circuits at the first failure.
An async version is available via `SelectManyAsync`.

---

## Exception Handling (`Try`)

Converts code that may throw into an `AxisResult`:

```csharp
var result = AxisResult.Try(() => int.Parse(input));
// result.IsSuccess if input is parseable, IsFailure with InternalServerError otherwise

var custom = AxisResult.Try(
    () => int.Parse(input),
    ex => AxisError.ValidationRule($"PARSE_{input}"));
```

### Critical exceptions **always escape** (are never caught):

- `OperationCanceledException` / `TaskCanceledException` — cancellation propagates correctly.
- `StackOverflowException`, `OutOfMemoryException` — impossible to handle.
- `ThreadAbortException` — legacy, never catch.
- `NullReferenceException`, `ArgumentNullException` — **these are programmer bugs**; converting them into `AxisError` masks defects.

This is deliberate: `Try` exists for **domain errors** (parsing failed, network went down), not to hide bugs.

### Async versions

```csharp
AxisResult result = await AxisResult.TryAsync(() => httpClient.PostAsync(url, body));
AxisResult<User> user = await AxisResult.TryAsync(() => repo.LoadAsync(id));
```

---

## Combination and Aggregation

### `Combine` — joins N untyped `AxisResult`s

```csharp
AxisResult r1 = ValidateName(name);
AxisResult r2 = ValidateEmail(email);
AxisResult r3 = ValidateAge(age);
AxisResult all = AxisResult.Combine(r1, r2, r3);
// IsSuccess if all succeed; IsFailure with all errors concatenated
```

Overloads exist for `params AxisResult[]` and `IEnumerable<AxisResult>`.
Async versions: `CombineAsync(IEnumerable<Task<AxisResult>>)` and `CombineAsync(IEnumerable<ValueTask<AxisResult>>)`.

### `All<TValue>` — joins N typed `AxisResult<T>`s

```csharp
IEnumerable<AxisResult<User>> results = ids.Select(GetUser);
AxisResult<IReadOnlyList<User>> all = AxisResult.All(results);
// Success: list of users, in the same order
// Failure: all errors from all results
```

**Returns `IReadOnlyList<TValue>`** (materialized) — consumers have a guaranteed `.Count` and indexer.

### `Combine`/`All` vs `Zip`

- `Combine`/`All`: **aggregate** errors from all results (useful in validation when you want to surface everything at once).
- `Zip`: **short-circuits** at the first error (follows the ROP track).

---

## Recovery (`Recover`, `OrElse`)

### `Recover` — error becomes a value

```csharp
result.Recover(defaultUser);                    // fixed value
result.Recover(() => new User());               // factory
result.Recover(errs => BuildFallback(errs));    // uses the errors
```

### `RecoverWhen` — selective recovery

```csharp
// By predicate
result.RecoverWhen(
    errs => errs.All(e => e.IsTransient),
    errs => defaultUser);

// By type
result.RecoverWhen(AxisErrorType.NotFound, () => new User { IsGuest = true });

// By exact code
result.RecoverWhen("CACHE_MISS", () => LoadFromDb());
```

### `RecoverNotFound` — common shortcut

```csharp
result.RecoverNotFound(() => CreateDefaultUser());
// Recovers only if ALL errors are NotFound
```

### `OrElse` — alternative with fallback

```csharp
// Try cache, fall back to database on failure
result.OrElse(errs => LoadFromDb(id));

// Accumulating errors from both paths
result.OrElse(errs => LoadFromDb(id), combineErrors: true);
// If the fallback also fails, returns errors from the first + errors from the second
```

### `RequireNotFound` — existence guard

Inverts the semantics of a lookup: "I **need** this to not exist".

- **Success (found)** → returns the provided error (entity already exists)
- **Failure with NotFound** → converts to `Ok()` (not found is what we wanted)
- **Failure with other errors** → propagates unchanged

```csharp
// "Create person" — require that the national ID is not already registered
readerPort.GetByNationalIdAsync(args.NationalId)
    .RequireNotFoundAsync(AxisError.BusinessRule("DOCUMENT_ID_ALREADY_ADDED"))
    .ThenAsync(() => CreateNewPersonEntity(args))
    .TapAsync(e => writePort.CreateAsync(e, args.NationalId))
    .MapAsync(NewInstance);
```

Note: `RequireNotFound` always returns `AxisResult` (non-generic) because the found value is discarded — the whole point is that the entity should **not** exist.

---

## Design Decisions

### 1. `CancellationToken` does **not** appear in combinators

`ThenAsync`, `MapAsync`, etc. do **not** accept a `CancellationToken`. Propagation is left to the caller via closure:

```csharp
await result.ThenAsync(u => repo.LoadAsync(u.Id, ct));
//                                          ^^ closure
```

The alternative of storing the CT in the result was considered and rejected — it complicates the signature of every delegate.

### 2. `Errors` is always non-null

`Errors` returns `Array.Empty<AxisError>()` when there are no errors. This simplifies consumers (always safe to iterate).

### 3. `Value` throws on failure

Alternatives considered:
- Return `default` / `null` — hides bugs.
- Return `Option<T>` — complicates the type with no real gain.

Throwing `NoAccessValueOnErrorResultException` with the error list in the message is a **diagnostic**. Accessing `Value` without checking `IsSuccess` is a usage error, not a normal scenario.

### 4. Error propagation without list copying

`Map<TNew>`, `Then<TNew>`, etc., on the failure path use `internal PropagateErrors<TNew>(source)`, which **shares** the internal list between results. This is safe because `_errors` is immutable after construction. Savings: 1 `List<>` + enumeration per stage on failure.

### 5. Extension methods for `Task<AxisResult<T>>`

Without them, async pipelines become unwieldy:

```csharp
// Without extensions
var user = await GetUserAsync(id);
if (user.IsFailure) return user;
var order = await user.ThenAsync(u => GetOrderAsync(u.Id));
if (order.IsFailure) return order;
var mapped = await order.MapAsync(o => Serialize(o));

// With extensions
var mapped = await GetUserAsync(id)
    .ThenAsync(u => GetOrderAsync(u.Id))
    .MapAsync(o => Serialize(o));
```