# AxisResult

**Railway-Oriented Programming for C# that actually works in production.**

A zero-dependency Result monad built for real-world .NET applications. No exceptions for control flow. No `null` checks scattered everywhere. No `try/catch` in your business logic. Just clean, composable pipelines that make your intent crystal clear.

```csharp
public Task<AxisResult<AddCellphoneResponse>> HandleAsync(AddCellphoneCommand cmd)
    => personFactory.GetByIdAsync(cmd.PersonId)
        .ThenAsync(person => cellphoneMediator.AddAsync(new() { CountryId = cmd.CountryId, Number = cmd.Number }))
        .ThenAsync(response => person.AddCellphoneAsync(response.CellphoneId))
        .ThenAsync(_ => unitOfWork.SaveChangesAsync())
        .MapAsync(_ => new AddCellphoneResponse { CellphoneId = response.CellphoneId });
```

Every operation either succeeds and flows forward, or fails and short-circuits. No nesting. No branching. No ambiguity.

---

## The Problem

This is what "enterprise C#" looks like in most codebases:

```csharp
public async Task<ApiResponse> Handle(CreateOrderCommand cmd)
{
    try
    {
        var customer = await _customerRepo.GetByIdAsync(cmd.CustomerId);
        if (customer == null)
            throw new NotFoundException("Customer not found");

        if (!customer.IsActive)
            throw new BusinessRuleException("Customer is not active");

        var existingOrder = await _orderRepo.GetByReferenceAsync(cmd.Reference);
        if (existingOrder != null)
            throw new ConflictException("Order already exists");

        var product = await _productRepo.GetByIdAsync(cmd.ProductId);
        if (product == null)
            throw new NotFoundException("Product not found");

        if (product.Stock < cmd.Quantity)
            throw new BusinessRuleException("Insufficient stock");

        try
        {
            var order = new Order(customer.Id, product.Id, cmd.Quantity);
            await _orderRepo.CreateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _eventBus.PublishAsync(new OrderCreatedEvent(order.Id));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish event");
                // Swallow? Rethrow? Who decides?
            }

            return new ApiResponse { OrderId = order.Id };
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictException("Duplicate order", ex);
        }
    }
    catch (NotFoundException ex) { return NotFound(ex.Message); }
    catch (BusinessRuleException ex) { return BadRequest(ex.Message); }
    catch (ConflictException ex) { return Conflict(ex.Message); }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error");
        return InternalServerError();
    }
}
```

**40 lines. 5 catch blocks. 3 null checks. 2 nested try/catch.** And the actual business intent is buried under defensive ceremony.

Exceptions are **goto statements in disguise**. They break your call stack, they're invisible in method signatures, they force you to guess what might fail, and they make every caller responsible for catching things that aren't exceptional at all. "Customer not found" isn't an exception — it's a perfectly normal outcome.

Now look at the same logic with AxisResult:

```csharp
public Task<AxisResult<CreateOrderResponse>> HandleAsync(CreateOrderCommand cmd)
    => customerFactory.GetByIdAsync(cmd.CustomerId)
        .ThenAsync(customer => orderFactory.CreateAsync(new()
        {
            CustomerId = customer.CustomerId,
            ProductId = cmd.ProductId,
            Quantity = cmd.Quantity
        }))
        .ThenAsync(_ => unitOfWork.SaveChangesAsync())
        .TapAsync(order => logger.LogInformation("Order {OrderId} created", order.OrderId))
        .MapAsync(order => new CreateOrderResponse { OrderId = order.OrderId });
```

**5 lines. Zero try/catch. Zero null checks. Zero exceptions.** Every possible failure is encoded in the return type. The pipeline reads like a sentence describing what the operation does.

That's Railway-Oriented Programming.

---

## What is Railway-Oriented Programming?

Imagine your code as a railway track with two rails:

```
Success ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━▶  Result
              ┃              ┃              ┃              ┃
           Validate       GetUser      CreateOrder      Save
              ┃              ┃              ┃              ┃
Failure ━━━━━━╋━━━━━━━━━━━━━━╋━━━━━━━━━━━━━━╋━━━━━━━━━━━━━━╋━▶  Errors
```

On the **success rail**, data flows from one operation to the next. Each operation transforms or validates the data, then passes it forward.

On the **failure rail**, errors propagate automatically. The moment any operation fails, all subsequent operations are **skipped** — no `if (result.IsFailure) return result;` boilerplate.

The magic is in the **switches** (the `┃` points). Operations like `ThenAsync`, `MapAsync`, and `EnsureAsync` are railway switches: they only execute when on the success rail. If you're already on the failure rail, they let you pass through untouched.

This isn't a new idea — it comes from functional programming (Haskell's `Either`, F#'s `Result`, Rust's `Result<T, E>`). But AxisResult is the first C# library that implements it **completely**, with full async support, zero dependencies, and APIs designed for how C# developers actually write code.

---

## Why AxisResult?

There are other Result libraries for C#. Here's why AxisResult is different.

### vs. FluentResults

FluentResults is popular but limited. It lacks monadic composition — you can't chain async operations without manual unwrapping. There's no `ValueTask` support, no tuple composition, no recovery patterns, and no typed error categories. It's a container, not a railway.

### vs. ErrorOr

ErrorOr offers basic chaining but misses the depth needed for production systems. No `ValueTask` variants, no `Zip` for combining independent operations, no `Recover`/`RecoverWhen` for conditional fallbacks, no `RequireNotFound` for idempotent creation patterns.

### vs. LanguageExt

LanguageExt is a 7.5MB functional programming framework. If you only need Result types, you're pulling in immutable collections, State monads, Reader monads, and an entirely non-idiomatic API. AxisResult gives you the composition power without the weight or the learning curve.

### vs. CSharpFunctionalExtensions

Solid library, but no `ValueTask` support, no `Zip`, no typed error categories, no recovery patterns, no parallel aggregation. It's good for basic Result patterns but falls short in complex domain scenarios.

### vs. Ardalis.Result

Designed for ASP.NET controllers, not for domain logic. Basic `Map`/`Bind` support, no composition depth, no async variants, no recovery. Great for HTTP response mapping, limited everywhere else.

### The Comparison

| Feature | AxisResult | FluentResults | ErrorOr | LanguageExt | CSharpFunctExt |
|---------|:---:|:---:|:---:|:---:|:---:|
| Monadic composition (Then/Map) | **Yes** | Partial | Yes | Yes | Yes |
| Task + ValueTask async | **Yes** | No | No | No | No |
| Tuple composition (Zip) | **Up to 4** | No | No | Yes | No |
| Conditional recovery (Recover/RecoverWhen) | **Yes** | No | No | No | No |
| Typed error categories | **12 types** | No | Partial | No | No |
| Transient error detection | **Yes** | No | No | No | No |
| RequireNotFound pattern | **Yes** | No | No | No | No |
| LINQ query syntax | **Yes** | No | Partial | Yes | Yes |
| Parallel aggregation (Combine/All) | **Yes** | No | No | Yes | No |
| Error accumulation (OrElse) | **Yes** | Partial | No | No | No |
| Zero external dependencies | **Yes** | Partial | Partial | No | Yes |
| Lightweight (< 50KB) | **Yes** | Yes | Yes | No (7.5MB) | Yes |

---

## Getting Started

### Installation

```
dotnet add package AxisTrix.Results
```

### Creating Results

```csharp
// Success
AxisResult success = AxisResult.Ok();
AxisResult<int> typed = AxisResult.Ok(42);

// Failure
AxisResult failure = AxisError.NotFound("USER_NOT_FOUND");     // implicit conversion
AxisResult<int> typed = AxisError.BusinessRule("INSUFFICIENT_STOCK");

// From values (implicit)
AxisResult<string> name = "John";   // auto-wraps in Ok

// From exceptions (safe boundary)
AxisResult result = AxisResult.Try(() => riskyOperation());
AxisResult<int> parsed = AxisResult.Try(() => int.Parse(input));
```

### Checking Results

```csharp
if (result.IsSuccess)
    Console.WriteLine($"Value: {result.Value}");

if (result.IsFailure)
    Console.WriteLine($"Errors: {result.JoinErrorCodes()}");

// Pattern matching
var message = result.Match(
    onSuccess: value => $"Got {value}",
    onFailure: errors => $"Failed: {errors[0].Code}"
);
```

### Chaining Operations

```csharp
// Each step only runs if the previous one succeeded
var result = await GetUserAsync(userId)
    .ThenAsync(user => ValidateOrder(user, productId))
    .ThenAsync(order => CalculateTotal(order))
    .MapAsync(total => new OrderResponse { Total = total });
```

---

## Real-World Examples

These are from a production codebase using AxisResult with Hexagonal Architecture + DDD + CQRS.

### Command Handler: Regenerate API Secret

A handler that loads an entity, updates its secret, persists, and invalidates the cache — all in one pipeline:

```csharp
internal class GenerateNewSecretHandler(
    IUnitOfWorkProvider uowProvider,
    IExternalApiAggregateApplicationFactory factory,
    ICachedSecretResolver cacheResolver
) : IAxisCommandHandler<GenerateNewSecretCommand, GenerateNewSecretResponse>
{
    public Task<AxisResult<GenerateNewSecretResponse>> HandleAsync(GenerateNewSecretCommand cmd)
    {
        var plainSecret = ExternalApiSecret.Generate();
        var hashedSecret = ExternalApiSecret.Hash(plainSecret);

        return factory.GetByIdAsync(cmd.ExternalApiId)           // Load entity (NotFound if missing)
            .ThenAsync(app => app.UpdateSecretAsync(hashedSecret)) // Domain operation (preserves entity)
            .ThenAsync(_ => uowProvider.UnitOfWork.SaveChangesAsync()) // Persist
            .ThenAsync(_ => cacheResolver.RemoveAsync(cmd.ExternalApiId)) // Invalidate cache
            .MapAsync(_ => new GenerateNewSecretResponse           // Transform to response
            {
                ExternalApiId = cmd.ExternalApiId,
                Secret = plainSecret
            });
    }
}
```

**Notice**: `ThenAsync` preserves the typed value. After `UpdateSecretAsync` returns `AxisResult` (no value), the pipeline still carries the original `IExternalApiAggregateApplication`. If any step fails, everything after it is skipped — including `SaveChangesAsync`.

### Query Handler: Get Entity by ID

Queries go directly from handler to port — no domain layer needed:

```csharp
internal class GetExternalApiByIdHandler(
    IExternalApisReaderPort readerPort
) : IAxisQueryHandler<GetExternalApiByIdQuery, GetExternalApiByIdResponse>
{
    public Task<AxisResult<GetExternalApiByIdResponse>> HandleAsync(GetExternalApiByIdQuery query)
        => readerPort.GetByIdAsync(query.ExternalApiId)
            .MapAsync(entity => new GetExternalApiByIdResponse
            {
                ExternalApiId = entity.ExternalApiId,
                Name = entity.ApiName
            });
}
```

If the entity doesn't exist, the reader port returns `AxisError.NotFound("EXTERNAL_API_NOT_FOUND")`. The `MapAsync` is never called. The error propagates cleanly to the caller.

### Authentication Handler: Verify Credentials

```csharp
internal class AuthenticateHandler(
    ICachedSecretResolver cachedResolver
) : IAxisCommandHandler<AuthenticateCommand>
{
    public Task<AxisResult> HandleAsync(AuthenticateCommand cmd)
        => cachedResolver.GetExternalApiAsync(cmd.ExternalApiId)
            .ThenAsync(app => app.VerifySecret(cmd.Secret));
}
```

**Two lines.** Load from cache, verify secret. If the API key doesn't exist: `NotFound`. If the secret is wrong: `Unauthorized`. The handler doesn't need to know or care about the details.

### Factory: Create with Duplicate Detection

The `RequireNotFound` pattern elegantly handles "create only if it doesn't exist":

```csharp
public Task<AxisResult<IPersonAggregateApplication>> CreateAsync(NewArgs args)
    => readerPort.GetByNationalIdAsync(args.NationalId)
        .RequireNotFoundAsync(AxisError.BusinessRule("DOCUMENT_ALREADY_EXISTS"))
        .WithValueAsync(new PersonEntity(true, PersonId.New, args.DisplayName, args.PictureProxyUrl, args.OriginId))
        .MapAsync(NewInstance)
        .ActionAsync(app => app.IsValidAsync())
        .ThenAsync(writePort.CreateAsync);
```

Here's what happens:
1. **Try to find** an existing person by national ID
2. **`RequireNotFoundAsync`**: If found, fail with `DOCUMENT_ALREADY_EXISTS`. If not found, continue (success!)
3. **`WithValueAsync`**: Create the new entity
4. **`MapAsync`**: Wrap in the application layer
5. **`ActionAsync`**: Run domain validation (phone format, document rules) — preserves the value on success
6. **`ThenAsync`**: Persist to database

### Factory: Load with Domain Validation

When an entity has invariants that need validation on load:

```csharp
public Task<AxisResult<ICellphoneAggregateApplication>> GetByIdAsync(CellphoneId id)
    => readerPort.GetByIdAsync(id)
        .MapAsync(NewInstance)
        .ActionAsync(app => app.IsValidAsync());  // Validate after hydration
```

`ActionAsync` runs domain validation and preserves the original value on success. If validation fails, the errors propagate.

### Aggregate Application: Domain + Persistence

The application layer coordinates domain rules with infrastructure:

```csharp
internal class PersonAggregateApplication(
    IPersonEntityProperties properties,
    IPersonCellphonesWriter cellphonesWriter
) : PersonEntity(properties), IPersonAggregateApplication
{
    public Task<AxisResult> AddCellphoneAsync(CellphoneId cellphoneId)
        => AddCellphone()   // Domain rule: checks if person is active
            .TapAsync(() => cellphonesWriter.AddCellphoneAsync(PersonId, cellphoneId));
}
```

`AddCellphone()` is a domain rule returning `AxisResult`. If the person isn't active, it returns `AxisError.BusinessRule("PERSON_NOT_ACTIVE")` and `TapAsync` never executes.

### Repository: Database Access Without Exceptions

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

The only `try/catch` in the entire architecture lives at the infrastructure boundary. Database exceptions are converted to `AxisResult` once, at the edge. Everything above — handlers, factories, domain rules — is exception-free.

### Validator: Automatic Pipeline Validation

Validators run automatically before the handler (via pipeline behavior):

```csharp
internal class AuthenticateValidator : AxisValidatorBase<AuthenticateCommand>
{
    public AuthenticateValidator()
    {
        RequiredTryParse(x => x.ExternalApiId, "EXTERNAL_API_ID_NOT_VALID",
            x => ExternalApiId.TryParse(x, out _));
        NotNullOrEmpty(x => x.Secret, "SECRET_NULL_OR_EMPTY");
    }
}
```

If validation fails, the handler is **never called**. Errors are returned as `AxisResult` with `AxisErrorType.ValidationRule`.

---

## Combining Independent Operations

### Zip: Build Tuples from Multiple Results

When you need values from multiple independent operations:

```csharp
var result = await GetUserAsync(userId)
    .ZipAsync(user => GetAccountAsync(user.AccountId))     // (User, Account)
    .ZipAsync((user, account) => GetPlanAsync(account.PlanId))  // (User, Account, Plan)
    .MapAsync((user, account, plan) => new DashboardResponse
    {
        UserName = user.Name,
        AccountBalance = account.Balance,
        PlanName = plan.Name
    });
```

Each `ZipAsync` adds a value to the tuple. If any step fails, the whole chain short-circuits. The final `MapAsync` destructures the tuple cleanly — no `.Value1`, `.Value2` needed.

### Combine: Validate Multiple Things in Parallel

```csharp
var result = AxisResult.Combine(
    ValidateName(cmd.Name),
    ValidateEmail(cmd.Email),
    ValidateAge(cmd.Age)
);
// Collects ALL errors, not just the first one
```

### All: Aggregate Typed Results

```csharp
var result = await AxisResult.AllAsync(
    userIds.Select(id => GetUserAsync(id))
);
// AxisResult<IReadOnlyList<User>> — all users, or all errors
```

---

## Recovery and Fallbacks

### Recover: Provide a Default

```csharp
var settings = await GetUserSettingsAsync(userId)
    .RecoverNotFound(() => DefaultSettings.Create());
```

### RecoverWhen: Conditional Recovery

```csharp
var data = await FetchFromPrimaryAsync()
    .RecoverWhen(AxisErrorType.ServiceUnavailable, () => FetchFromFallbackAsync());
```

### OrElse: Try an Alternative

```csharp
var user = await FindByEmailAsync(email)
    .OrElseAsync(_ => FindByUsernameAsync(email));  // Try username if email fails
```

### OrElse with Error Accumulation

```csharp
var user = await FindByEmailAsync(email)
    .OrElseAsync(_ => FindByPhoneAsync(phone), combineErrors: true);
// If both fail, you get errors from BOTH attempts
```

---

## Error Categories

Every `AxisError` has a typed category:

```csharp
AxisError.NotFound("USER_NOT_FOUND")             // → 404
AxisError.ValidationRule("EMAIL_INVALID")         // → 400
AxisError.BusinessRule("INSUFFICIENT_BALANCE")    // → 422
AxisError.Conflict("ORDER_ALREADY_EXISTS")        // → 409
AxisError.Unauthorized("INVALID_TOKEN")           // → 401
AxisError.Forbidden("ADMIN_ONLY")                 // → 403
AxisError.InternalServerError("DB_FAILURE")       // → 500
AxisError.ServiceUnavailable("API_DOWN")          // → 503
AxisError.Timeout("GATEWAY_TIMEOUT")              // → 504
AxisError.TooManyRequests("RATE_LIMITED")          // → 429
AxisError.GatewayTimeout("UPSTREAM_TIMEOUT")       // → 504
AxisError.Mapping("INVALID_DATA_FORMAT")           // → 500
```

### Transient Detection

```csharp
if (error.IsTransient)   // true for ServiceUnavailable, Timeout, TooManyRequests, GatewayTimeout
    await RetryAsync();  // Safe to retry
```

This is built into the type system. Circuit breakers, retry policies, and health checks can inspect `IsTransient` without parsing error messages or maintaining string lists.

### Cross-Layer Error Transformation

```csharp
var result = await internalService.ProcessAsync()
    .MapErrorAsync(error => AxisError.InternalServerError($"PROCESSING_{error.Code}"));
```

Remap error codes and types as they cross architectural boundaries.

---

## LINQ Query Syntax

For developers who prefer comprehension syntax:

```csharp
AxisResult<decimal> total =
    from customer in GetCustomerAsync(customerId)
    from order in CreateOrderAsync(customer.Id, productId)
    select order.Total;
```

Equivalent to:

```csharp
var total = await GetCustomerAsync(customerId)
    .ThenAsync(customer => CreateOrderAsync(customer.Id, productId))
    .MapAsync(order => order.Total);
```

Both styles are first-class. Use whichever reads better for your team.

---

## ValueTask: Zero-Allocation Async

Every async method has both `Task` and `ValueTask` variants:

```csharp
// Task (standard)
public Task<AxisResult<User>> GetUserAsync(UserId id) => ...;

// ValueTask (hot path, zero allocation when synchronous)
public ValueTask<AxisResult<User>> GetUserAsync(UserId id) => ...;
```

On hot paths where the result is often cached or synchronous, `ValueTask` avoids heap allocations entirely. All composition methods (`ThenAsync`, `MapAsync`, `TapAsync`, etc.) work identically with both.

---

## API Reference

### Creating Results

| Method | Description |
|--------|-------------|
| `AxisResult.Ok()` | Success with no value |
| `AxisResult.Ok(value)` | Success with a value |
| `AxisResult.Error(error)` | Failure from a single error |
| `AxisResult.Error(errors)` | Failure from multiple errors |
| `AxisResult.Try(action)` | Wrap an action that might throw |
| `AxisResult.Try(func)` | Wrap a function that might throw |
| `AxisResult.TryAsync(action)` | Async version of Try |
| `AxisResult.TryAsync(func)` | Async version of Try with return value |
| Implicit: `value` | Assign any value where `AxisResult<T>` is expected |
| Implicit: `AxisError` | Assign an error where `AxisResult` is expected |

### Transforming (Success Rail)

| Method | Signature | Description |
|--------|-----------|-------------|
| `Map` | `T -> TNew` | Transform the value (pure, cannot fail) |
| `MapAsync` | `T -> Task<TNew>` | Async transform |
| `Then` | `T -> AxisResult<TNew>` | Chain to a failable operation returning a new value |
| `Then` | `T -> AxisResult` | Chain to a failable side effect, **preserves original value** |
| `ThenAsync` | Async versions of both `Then` overloads | |

### Side Effects

| Method | Description |
|--------|-------------|
| `Tap(action)` | Run side effect on success, return original result |
| `TapAsync(func)` | Async side effect on success |
| `TapError(action)` | Run side effect on failure (logging, metrics) |
| `TapErrorAsync(func)` | Async side effect on failure |

### Validation

| Method | Description |
|--------|-------------|
| `Ensure(predicate, error)` | Guard clause — fails if predicate is false |
| `Ensure(func)` | Delegated validation — func returns `AxisResult` |
| `EnsureAsync` | Async versions |
| `ActionAsync(func)` | Failable async validation that preserves value |

### Combining Values

| Method | Description |
|--------|-------------|
| `Zip(func)` | Combine current value with a new one into a tuple |
| `ZipAsync(func)` | Async version |
| Chained: `.Zip().Zip()` | Build tuples up to `(T1, T2, T3, T4)` |
| `MapAsync((a, b) => ...)` | Destructure tuples in the mapper |

### Aggregation

| Method | Description |
|--------|-------------|
| `Combine(results...)` | Join N results — collects **all** errors |
| `CombineAsync(tasks)` | Async version |
| `All(results)` | Join N typed results into `IReadOnlyList<T>` |
| `AllAsync(tasks)` | Async version |

### Recovery and Fallbacks

| Method | Description |
|--------|-------------|
| `Recover(value)` | On failure, replace with a default |
| `Recover(func)` | On failure, compute a fallback |
| `RecoverWhen(predicate, func)` | Recover only if errors match a condition |
| `RecoverWhen(AxisErrorType, func)` | Recover only for a specific error type |
| `RecoverWhen(code, func)` | Recover only for a specific error code |
| `RecoverNotFound(func)` | Recover only if all errors are `NotFound` |
| `OrElse(fallback)` | Try an alternative operation |
| `OrElse(fallback, combineErrors: true)` | Alternative with error accumulation |

### Existence Guards

| Method | Description |
|--------|-------------|
| `RequireNotFound(error)` | Found → error, NotFound → Ok, other errors → propagate |
| `RequireNotFoundAsync(error)` | Async version |
| `WithValueAsync(value)` | Promote `AxisResult` to `AxisResult<T>` with a value |

### Error Transformation

| Method | Description |
|--------|-------------|
| `MapError(func)` | Transform each error individually |
| `MapError(func<list>)` | Transform/filter the entire error list |
| `MapErrorAsync` | Async versions |

### Terminal

| Method | Description |
|--------|-------------|
| `Match(onSuccess, onFailure)` | Convert to a final type — runs exactly one branch |
| `MatchAsync` | Async version |

### LINQ

| Syntax | Equivalent |
|--------|------------|
| `from x in result select f(x)` | `result.Map(f)` |
| `from x in r1 from y in r2 select ...` | `r1.Then(x => r2).Map(...)` |
| `SelectManyAsync` | Async LINQ chaining |

### Conversion

| Method | Description |
|--------|-------------|
| `AsTaskAsync()` | Wrap sync result in `Task` |
| `AsValueTaskAsync()` | Wrap sync result in `ValueTask` |

---

## Design Principles

1. **Errors are values, not exceptions.** An operation that can fail says so in its return type. No surprises, no invisible control flow.

2. **The type system is the documentation.** When you see `Task<AxisResult<User>>`, you know exactly what can happen: you'll get a User, or you'll get errors. No guessing.

3. **Composition over ceremony.** Small, focused operations that compose into complex workflows. Each piece is testable in isolation.

4. **Fail fast, recover deliberately.** Errors propagate automatically (fail fast), but recovery is always explicit (`Recover`, `RecoverWhen`, `OrElse`).

5. **Exceptions at the boundary, results everywhere else.** Use `AxisResult.Try()` at infrastructure edges (HTTP clients, databases, file I/O). Everything above that is exception-free.

---

## License

Apache 2.0

---

*AxisResult turns your error handling from a liability into a feature. Every failure path is visible, testable, and composable. Your future self will thank you.*
