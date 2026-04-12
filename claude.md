# AxisTrix — Guide for AI Agents

> **Everything you do must have a clear reference in this file for how to do it architecturally.**
> If there is no scaffold or clear documentation on how to proceed, ask the developer how to do it and update this document.

---

## Critical workflow rules

- **Zero errors/warnings**: Whenever you finish something, run the build and the tests. If fixing the warning/error is out of scope for the task, surface the problem to resolve together with the developer. If it is in scope, fix it before marking the task as done.
- **American English**: All naming (classes, methods, variables, files, comments, etc.) must be in American English, even if the prompt is in another language.
- **Before committing**: Flag any non-American English or non-compliant code you find—notify the developer once the processing is complete.
- **Controllers require E2E tests**: When creating or modifying a WebApi Controller, always create/update corresponding E2E integration tests using TestContainers. Create a logical sequence of tests and use the results to ensure the "happy path" and other important paths.
- **No undocumented implementations**: If something has no scaffold or documentation, ask the developer before implementing.

---

## Architecture Overview

**Hexagonal DDD + CQRS + Vertical Slices.** No layer folders — everything organized by subdomain/context.

| Concept | Definition                                                                                                                                                                                                                     |
|---------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Subdomain** | Core, Support, or Generic. Full hexagonal unit.                                                                                                                                                                                |
| **Bounded Context (BC)** | Fully decoupled. Communication via SDK (sync) or IAxisBus events (async). A BC NEVER accesses another BC's repos, services, or ports directly. Sometimes, a BC can be a full hexagonal unit for microservice purposes.         |
| **Aggregate** | Group of entities. Only accessible through its Application layer.                                                                                                                                                              |
| **Use Case** | A Command or Query executed by a user, internal system, or external system. A command/query is a request, and a command may or may not have a response. A response cannot be null; it returns a 'NotFound' AxisResult instead. |
| **Port** | An interface for external technology or for the injection of adapters via dependency injection.                                                                                                                                                         |
| **SDK** | Interface for other BCs or external APIs. Can be in-process, HTTP, or gRPC.                                                                                                                                                    |

---

## Project Structure

```
{Domain}/
└── {SubDomain}/
    ├── Contracts/{SubDomain}.Contracts/
    │   ├── {BC}/{Feature}/{UseCase}/v1/I{Feature}Mediator       # Interface for SDK Implementation
    │   └── {BC}/{Feature}/{UseCase}/v1/                         # If the feature has the same name as the BC (CRUDs), there is no feature folder
    ├── Core/
    │   ├── {SubDomain}.SharedKernel/                            # Entity interfaces, Value Objects, ApplicationConfig
    │   ├── {SubDomain}.Domain/                                  # Entities (Properties + Rules, partial classes)
    │   └── {SubDomain}.Application/                             # Handlers, Validators, Factories, AggregateApplications
    │       ├── {BC}/                                            # DependencyInjection, AggregateApplicationFactory, AggregateApplications
    │       └── {BC}/UseCases/v1/{SameContractStructure}/        # Handlers and Validators
    ├── Ports/{SubDomain}.Ports/                                 # Reader/Writer port interfaces, IUnitOfWorkProvider
    ├── Adapters/
    │   ├── Driven/
    │   │   ├── Repositories/{SubDomain}.Repository.[Technology]/   # Postgres | MongoDb | etc.
    │   │   ├── Producers/{SubDomain}.Producers.[Technology]/       # Kafka | RabbitMq | etc.
    │   │   ├── Consumers/{SubDomain}.Consumers.[Technology]/       # Kafka | RabbitMq | etc.
    │   │   └── Others/
    │   └── Driving/
    │       ├── SDK/{SubDomain}.SDK.Application/                    # SDK for Dependency Injection
    │       ├── SDK/{SubDomain}.SDK.HttpClient/                     # SDK for HTTP Communication
    │       ├── SDK/{SubDomain}.SDK.GrpcClient/                     # SDK for gRPC Communication
    │       ├── {SubDomain}.WebApi/                                 # Web Api for microservices
    │       └── {SubDomain}.Grpc/                                   # gRPC for microservices
    └── Tests/
        ├── {SubDomain}.UnitTests/
        └── {SubDomain}.IntegrationTests/
```

If the architecture is not microservices-based, there will be no integration with Web API or gRPC. The monolith will use the SDK.Application to execute the use cases.

**Project naming**: `{SubDomain}.{Layer}` — e.g., `IdentityTrix.Application`, `IdentityTrix.Repository.Postgres`

---

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Command | `<UseCase>Command` | `AddCellphoneToPersonCommand` |
| Query | `Get<Intent><ByFilter>Query` / `List<Intent><ByFilter>Query` | `GetPersonByEmailQuery`, `ListTokensQuery` |
| Response | `<Command/QueryName>Response` (NEVER `CommandResponse`/`QueryResponse`) | `GetPersonByEmailResponse` |
| Handler | `<Command/QueryName>Handler` | `RequestAuthCodeByEmailHandler` |
| Validator | `<Command/QueryName>Validator` | `AddCellphoneToPersonValidator` |
| Entity | `{Name}Entity` split into `{Name}EntityProperties.cs` + `{Name}EntityRules.cs` | `PersonEntityProperties.cs` |
| Entity Interface | `I{Name}EntityProperties` | `IPersonEntityProperties` |
| Port Reader | `I{Entities}ReaderPort` or `I{Entities}Reader` — **{Entities} is the entity name in plural** | `IPersonsReaderPort`, `IExternalApisReaderPort` (never `IPersonReaderPort`) |
| Port Writer | `I{Entities}WritePort` or `I{Entities}Writer` — **{Entities} is the entity name in plural** | `IPersonsWritePort`, `IExternalApisWritePort` (never `IPersonWritePort`) |
| Repository | `{Entities}Repository` | `PersonsRepository` |
| DbEntity | `{Entity}DbEntity` | `PersonDbEntity` |
| Table Constants | `{Entities}Table` | `PersonsTable` |
| Factory Interface | `I{Aggregate}AggregateApplicationFactory` | `IPersonAggregateApplicationFactory` |
| Application Interface | `I{Aggregate}AggregateApplication` | `IPersonAggregateApplication` |
| SDK Interface | `I{Feature}Mediator` | `IPersonsMediator` |
| ApplicationConfig key | PascalCase, no dots | `AppKey = "IdentityTrix"` |
| Error Code | UPPER_SNAKE_CASE | `PERSON_NOT_ACTIVE`, `AUTH_CODE_NOT_VALID` |
| DB Schema | UPPER_SNAKE_CASE | `IDENTITY_TRIX_PERSONS` |
| DB Table | `{SCHEMA}.{NAME}` UPPER_SNAKE_CASE | `IDENTITY_TRIX_PERSONS.PERSONS` |
| DB Column | UPPER_SNAKE_CASE | `PERSON_ID`, `DISPLAY_NAME` |
| Migration Version | `V1`, `V2`, ... | `("V1", V1)` |

---

## Access Modifiers

| Component                                                  | Modifier |
|------------------------------------------------------------|----------|
| Commands, Queries, Responses                               | `public record` |
| Validators                                                 | `internal class` |
| Handlers                                                   | `internal class` |
| Entities                                                   | `internal partial class` |
| DbEntities                                                 | `internal record` |
| Repositories, Factories, Application, SDK Mediators (impl) | `internal class` |
| Ports, BC Mediators (interfaces), SharedKernel interfaces  | `public interface` |
| DI extensions (public modules)                             | `public static class` |
| DI extensions (internal modules)                           | `internal static class` |
| Value Objects                                              | `readonly partial record struct` |

---

## Code Rules

- Primary constructors for DI (never fields + explicit constructor)
- `{ get; init; }` in Commands/Queries/Responses; `{ get; }` in Entities; `{ get; private set; }` when mutation is needed
- `required` on mandatory Response properties
- `nullable` properties on Requests (Commands/Queries)
- NEVER `throw` in application code — use AxisResult
- NEVER `try/catch` in handlers
- NEVER `if/else` for AxisResult flow control — use monadic operators
- NEVER use an ORM — raw parameterized SQL with the technology implemented (Npgsql, MongoDb C# driver, etc).
- NEVER add `SaveChangesAsync()` in the Application layer, only on Handlers
- NEVER access Domain Entity directly — only through Application layer
- NEVER add packages without asking the developer
- NEVER define classes/records/interfaces as `public` except if in the Access Modifiers table.

**!!!Key principle!!!**: Use cases trigger handlers, which orchestrate ports and aggregate applications. These applications are created by a factory and orchestrates domain rules using reader/writer ports. Nothing is saved until the handler calls `UnitOfWork.SaveChangesAsync()`. The Aggregate Application NEVER manages UoW.

---

## CQRS

### Commands
- `record` implementing `IAxisCommand<TResponse>` or `IAxisCommand` (no response)
- Use primitive types (`string?`, `int?`) for externally-exposed commands

### Queries
- `record` implementing `IAxisQuery<TResponse>`

### Handlers
- Return `Task<AxisResult<TResponse>>` or `Task<AxisResult>`
- Use functional composition
- Manage UoW
- Queries go directly handler → port (never through Application/Domain)

### AxisResult Composition Methods

#### Creating Results
                                 
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

#### Transforming & Chaining      
                                 
| Method | When to Use |         
|--------|-------------|         
| `.Map(func)` | Transform the success value (pure, cannot fail): `User → string` |                
| `.MapAsync(func)` | Async version of `Map` |                   
| `.Then(func)` | Chain to an operation that **may fail** and returns a **new value** (`AxisResult<TNew>`): `User → AxisResult<Order>` |
| `.Then(func)` | Chain to a **failable side effect** (`AxisResult`) that **preserves the original value** — propagates errors if the operation fails, returns the original `AxisResult<T>` on success |
| `.ThenAsync(func)` | Async version of `Then` (both overloads) |                  
| `.Ensure(predicate, error)` | Guard clause — fails with `error` if predicate is false |          
| `.Ensure(func)` | Delegated validation — `func` returns `AxisResult`, fails if that result fails |                                
| `.EnsureAsync(predicate, error)` | Async version of `Ensure` |  
| `.WithValueAsync(value)` | Promote an untyped `AxisResult` to `AxisResult<T>` by attaching a fixed value on success — use when the previous step has no return value but the next step needs one |

#### Combining Values (Zip)       
                                 
| Method | When to Use |         
|--------|-------------|         
| `.Zip(func)` | Combine current value with a new one into a tuple `(T1, T2)` — pure mapper or failable (`AxisResult<TNew>`) |      
| `.ZipAsync(func)` | Async version of `Zip` |                    
| `.Zip(...).Zip(...)` | Chain zips to build `(T1, T2, T3)` up to `(T1, T2, T3, T4)` |             
| `.MapAsync((a, b) => ...)` | Transform a tuple after `Zip` — deconstructed params, no `.Value1`/`.Value2` needed |                
| `.MapAsync((a, b, c) => ...)` | Same for 3-element tuples |     
| `.MapAsync((a, b, c, d) => ...)` | Same for 4-element tuples |  

#### Side Effects                 
                                 
| Method | When to Use |         
|--------|-------------|         
| `.Tap(action)` | Execute side effect on success (logging, metrics), returns original result |    
| `.Tap(action<TValue>)` | Same, with access to the value |       
| `.TapAsync(func)` | Async side effect on success |              
| `.TapError(action)` | Execute side effect on failure (logging errors) |                          
| `.TapErrorAsync(func)` | Async version of `TapError` |          

#### Aggregating Multiple Results 
                                 
| Method | When to Use |         
|--------|-------------|         
| `AxisResult.Combine(r1, r2, r3)` | Join N untyped results — collects **all** errors (validation-style) |                          
| `AxisResult.CombineAsync(tasks)` | Async version of `Combine` (Task or ValueTask) |              
| `AxisResult.All(results)` | Join N typed `AxisResult<T>` → `AxisResult<IReadOnlyList<T>>` — collects all errors |                 
| `AxisResult.AllAsync(tasks)` | Async version of `All` (Task or ValueTask) |                      

#### Existence Guard

| Method | When to Use |
|--------|-------------|
| `.RequireNotFound(error)` | Invert a lookup: **success (found) → error**, **NotFound → Ok()**, other errors propagate. Use for "create if not exists" flows |
| `.RequireNotFoundAsync(error)` | Async extension (Task/ValueTask) version of `RequireNotFound` |

#### Recovery & Fallback          
                                 
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

#### Error Transformation         
                                 
| Method | When to Use |         
|--------|-------------|         
| `.MapError(func<AxisError, AxisError>)` | Transform each error individually (e.g., remap codes between layers) |                  
| `.MapError(func<errors, IEnumerable>)` | Transform/filter the entire error list |               
| `.MapErrorAsync(func)` | Async version of `MapError` |          

#### Branching & Terminal         
                                 
| Method | When to Use |         
|--------|-------------|         
| `.Match(onSuccess, onFailure)` | Convert result to a final type (e.g., `IActionResult`) — always runs exactly one branch |        
| `.MatchAsync(onSuccess, onFailure)` | Async version of `Match` |

#### LINQ Query Syntax            
                                 
| Method | When to Use |         
|--------|-------------|         
| `from x in result select ...` | LINQ `Select` — same as `Map` | 
| `from x in r1 from y in r2 select ...` | LINQ `SelectMany` — same as chained `Then`, short-circuits on first failure |            
| `.SelectManyAsync(binder, projector)` | Async LINQ-style chaining |                              

#### Async Conversion             
                                 
| Method | When to Use |         
|--------|-------------|         
| `.AsTaskAsync()` | Wrap a sync `AxisResult` into `Task<AxisResult>` |                            
| `.AsValueTaskAsync()` | Wrap a sync `AxisResult` into `ValueTask<AxisResult>` |                  
                                 
> **Task vs ValueTask**: every combinator above exists in both `Task` and `ValueTask` variants. Use `ValueTask` on hot paths to avoid allocations; use `Task` when you need `Task.WhenAll` or similar APIs.                            

#### AxisError Properties         
                                 
| Property | Description |       
|----------|-------------|       
| `.Code` | Stable string identifier (e.g., `"USER_NOT_FOUND"`) | 
| `.Type` | `AxisErrorType` enum category |                       
| `.IsTransient` | `true` for `ServiceUnavailable`, `Timeout`, `TooManyRequests`, `GatewayTimeout` — safe to retry |                
                                 
---               

### Validators
- Extend `AxisValidatorBase<TCommand>`
- Rules in constructor; error codes in UPPER_SNAKE_CASE
- Run automatically before the handler (via `ValidationBehavior`)

#### Available Methods

| Method | Signature | Purpose |
|--------|-----------|---------|
| `NotNullOrEmpty` | `(expression, errorCode)` | Validates that the property is not null, not default, and not whitespace (for strings) |
| `NotNullOrEmpty` | `(expression, errorCode, Action dependentRules)` | Validates not-null, then runs `dependentRules` only when the value is present (conditional block) |
| `NotNullOrEmpty` | `(expression, errorCode, Action<TProperty> dependentRules)` | Validates not-null, then passes the **non-null value** into `dependentRules` for chained validation |
| `RequiredWithMaxLength` | `(expression, errorCode, length?)` | Not-null + max length check (default 255) |
| `RequiredEmail` | `(expression, errorCode)` | Not-null + email format |
| `RequiredGuid7` | `(expression, errorCode)` | Not-null + valid UUID v7 |
| `RequiredTryParse` | `(expression, errorCode, Func<object?, bool> parse)` | Not-null + custom parse predicate (e.g., Value Object `TryParse`) |
| `RequiredCellPhone` | `(expression, countryId, errorCode)` | Not-null + phone format for the given `CountryId` |
| `DocumentId` | `(expression, countryId, errorCode)` | Not-null + document validation by country (e.g., CPF for Brazil) |
| `DocumentId` | `(expression, Func<T, CountryId?> countrySelector, errorCode)` | Same, but resolves `CountryId` dynamically from another property |

#### Dependent Rules Pattern

The third overload of `NotNullOrEmpty` enables **chained conditional validation** — validate a property first, then use its resolved value to validate another property. This is the primary pattern for cross-field validation:

```csharp
// 1. Validate CountryId exists via lookup → if valid, use the resolved CountryId to validate phone format
NotNullOrEmpty(x => CountryIds.GetById(x.CountryId), "COUNTRY_ID_NULL_OR_NOT_VALID",
    countryId => RequiredCellPhone(x => x.CellphoneNumber, countryId!.Value, "CELLPHONE_NUMBER_NULL_OR_NOT_VALID"));

// 2. Validate CountryId via TryParse first, then chain document validation using a selector
RequiredTryParse(x => x.CountryId, "COUNTRY_ID_NULL_OR_NOT_VALID", value => CountryId.TryParse(value?.ToString(), out _));
DocumentId(x => x.DocumentId, instance => CountryId.Parse(instance.CountryId), "DOCUMENT_ID_NULL_OR_NOT_VALID");
```

**How it works**: `NotNullOrEmpty(expr, errorCode, Action<TProperty>)` first validates the expression is not null. Only if it passes, it invokes the lambda with the **already-validated non-null value**, allowing you to safely use it as input for the next validation rule (e.g., passing a resolved `CountryId` into `RequiredCellPhone`).

---

## Domain Entities

Each entity is split into two partial class files:

**`{Entity}EntityProperties.cs`** — `internal partial class` with primary constructor, implements `I{Entity}EntityProperties`, immutable `{ get; }` properties. Secondary constructor accepts the interface for rehydration.

**`{Entity}EntityRules.cs`** — `internal partial class`, business methods returning `AxisResult`. Private methods for internal rules; public methods compose private ones. Static methods for value generation.

**SharedKernel interface** — `I{Entity}EntityProperties` defines the read contract. Can have computed properties (e.g., `bool IsActive => WasAuthenticated && !CanceledAt.HasValue`).

Root entities: `{Aggregate}/Root/`. Non-root: `{Aggregate}/Entities/`.

---

## Application Layer (Factory + AggregateApplication)

**Core rules**:
- Domain Entity NEVER accessed directly — Application inherits from Entity and is exposed via interface
- Application Factory receives application injections (Ports/AxisMediator)

**Factory** (`I{Name}AggregateApplicationFactory`):
- Methods: `GetByIdAsync()` and `CreateAsync(NewArgs)` where `NewArgs` is a nested record
- `GetByIdAsync()` → reader port + `.MapAsync()` to wrap in Application
- `CreateAsync()` → use `.RequireNotFound()` for duplicate detection, then `.TapAsync(writePort.CreateAsync)`
- `NewInstance(I{Entity}EntityProperties properties)` — private helper that constructs the Application by passing `properties` directly to the AggregateApplication constructor (never wrap in `new {Entity}Entity(properties)` first — the AggregateApplication's base call handles the rehydration)

**AggregateApplication** (`I{Name}AggregateApplication : I{Entity}EntityProperties`):
- `internal class {Name}AggregateApplication(I{Entity}EntityProperties properties, ...) : {Entity}(properties), I{Name}AggregateApplication`
- The first constructor parameter is always `I{Entity}EntityProperties properties` — never the concrete `{Entity}Entity` type. The base call `: {Entity}(properties)` rehydrates the entity from the interface via its secondary constructor.
- Coordinates: call the inherited domain rule directly (no `root.` prefix) → `.TapAsync(() => {child}Writer.{Action}())`. Example: `AddCellphone().TapAsync(() => cellphonesWriter.AddCellphoneAsync(PersonId, cellphoneId))`.

**DI**: `AddScoped<I{Name}AggregateApplicationFactory, {Name}AggregateApplicationFactory>()`

Interface and implementation live in the **same `.cs` file** for both Factory and AggregateApplication files.

---

## Ports

- **Entity ports always use the plural form of the entity name**: `I{Entities}ReaderPort` / `I{Entities}WritePort`. Example: for a `Person` entity, the ports are `IPersonsReaderPort` and `IPersonsWritePort` — never `IPersonReaderPort`/`IPersonWritePort`. This reflects that a port exposes operations over the *collection* of entities, not over a single one.
- Reader: `I{Entities}ReaderPort` — return `Task<AxisResult<I{Entity}EntityProperties>>`, never nullable
- Writer: `I{Entities}WritePort` — return `Task<AxisResult>`, accept `I{Entity}EntityProperties`
- `IUnitOfWorkProvider` — `IUnitOfWork UnitOfWork { get; }`
- Parameters use SharedKernel types; NEVER throw exceptions in port implementations

---

## Repositories (Postgres)

- Extend `TechnologyRepositoryBase(mediator, logger, uow)`
- Constructor: `IAxisMediator mediator, IAxisLogger<{Technology}RepositoryBase> logger, [FromKeyedServices(ApplicationConfig.AppKey)] I{Technology}UnitOfWork uow`
- Implement Reader AND Writer ports in the same class when both use the same database
- Base class methods: `ExecuteAsync()`, `GetAsync<T>()`, `ListAsync<T>()`
- Raw parameterized SQL with table constants; `SelectColumns` as `const string`
- Each BC has its own schema (e.g., `IDENTITY_TRIX_PERSONS`)

**DbEntity** — `internal record : I{Entity}EntityProperties` with `static FromReader(NpgsqlDataReader)` for ordinal mapping.

**Table constants** — `static class` with `const string Table`, column constants, and DDL as `const string V1`.

**DI per aggregate** — each root aggregate folder has its own `DependencyInjection.cs` registering root + all child repos as Scoped.

**Child entities** go under `ChildrenEntities/{Resource}/`.

---

## Database Migrations

**`DatabaseScripts`** — `public static class` with `const string Schema` and `internal static readonly (string Version, string Script)[] Migrations`.

**Table DDL** — each Table class defines `const string V1` (and `V2`, `V3` for incremental changes).

**Migration runner** — `{BC}Migrations.InitializePostgresAsync(string connectionString)`:
- Creates schema + `SCHEMA_MIGRATIONS` table (`VERSION VARCHAR(50) PK`, `APPLIED_AT TIMESTAMPTZ`)
- Applies migrations idempotently in a transaction

---

## SDK / Driving Adapters

- Public interface: `I{Name}Mediator` in `{Subdomain}.SDK`
- Internal DI implementation: `{Name}Mediator` in `{Subdomain}.SDK.Application`
- Primary constructor: `IAxisMediator mediator`
- Commands: `mediator.Cqrs.ExecuteAsync<TCommand, TResponse>(command)`
- Queries: `mediator.Cqrs.QueryAsync<TQuery, TResponse>(query)`
- SDK is a thin wrapper — zero logic, only delegates to CQRS pipeline
- SDK.Application DI calls `Add{Subdomain}Application()` — it is the entry point that wires everything

---

## Cross-Cutting Infrastructure Examples

| Component | Port Interface | Registration |
|-----------|---------------|-------------|
| Cache | `IAxisCache` | `AddMemoryCacheTrix()` → Singleton |
| Event Bus | `IAxisBus` + `IAxisEvent` + `IAxisEventHandler<T>` | `AddMemoryBusTrix()` → Singleton; handlers auto-discovered by `AddCqrsMediator()` |
| Object Storage | `IAxisStorage` | `AddCloudflareR2StorageTrix(settings)` → Singleton |

**Events**: `record : IAxisEvent`, past-tense name (e.g., `PersonCreatedEvent`). Published via `IAxisBus.PublishAsync<TEvent>(event, ct, topics)`. Multiple handlers per event run in parallel.

**Event Handlers**: `internal class : IAxisEventHandler<TEvent>`, return `Task<AxisResult>`, auto-discovered, registered as Transient.

---

## IAxisMediator

| Property | Purpose |
|----------|---------|
| `CancellationToken` | Used by handlers and repos — NEVER passed as parameter |
| `Cqrs` | CQRS dispatch |
| `TraceId` | Distributed tracing |
| `OriginId` | Request origin identifier (nullable) |
| `UserPersonData` | Authenticated person data (nullable) |
| `JourneyId` | Journey tracking (nullable) |
| `GetPersonFromCacheAsync(PersonId)` | Fetch person data from cache |

**IAxisMediatorContextAccessor** — Singleton with `AsyncLocal<T>`. Populated by `AxisMediatorContextAccessorMiddleware` from HTTP headers/claims. Properties: `OriginId`, `JourneyId`, `CancellationToken`, `PersonId`.

**IAxisMediatorAccessor** — Singleton ambient context. `AxisMediator` sets `_accessor.Mediator = this` on construction, clears on `Dispose()`.

---

## Dependency Injection

```csharp
services.InitAxisTrixAdd()
    .Add{Module}()
    .EndAxisTrixAdd()          // Production
    // .EndAxisTrixAdd_Mock()  // Tests
    .BuildServiceProvider();
```

`AddCqrsMediator(assembly)` — auto-discovers handlers, validators, and event handlers in the assembly.

### DI Lifetimes

| Component | Lifetime |
|-----------|----------|
| Repositories, Factories, Application Services, SDK Mediators, IUnitOfWorkProvider, IAxisMediator | Scoped |
| Handlers, Pipeline Behaviors, Event Handlers | Transient |
| DataSource (Npgsql), IAxisMediatorAccessor, IMediatorContextAccessor, IAxisCache, IAxisBus, IAxisStorage | Singleton |

### Keyed Services
`[FromKeyedServices(ApplicationConfig.AppKey)]` on repository constructors enables each Subdomain/BC to have its own AppKey and database connection.

---

## Pipeline Behaviors (outer → inner)

1. **TelemetryBehavior** — OpenTelemetry spans + metrics
2. **LoggingBehavior** — Logs request name + trace/journey IDs
3. **PerformanceBehavior** — Warns if execution exceeds 500ms
4. **ValidationBehavior** — Runs validator, returns errors without throw
5. **Handler** — Actual command/query execution

---

## Value Objects

### With Source Generator `[ValueObject]` (shared types in `AxisTrix.SourceGen`)

```csharp
[ValueObject(TryParse = false)]
public partial record PersonId
{
    public static PersonId New => new(Guid.CreateVersion7().ToString());
    private PersonId(string? value) { /* validate */ Value = value; }
    public string Value { get; }
}
```

| Option | Default | Generates |
|--------|---------|-----------|
| `PropertyName` | `"Value"` | Property name used in conversions |
| `ImplicitFromString` | `true` | `implicit operator Type(string?)` |
| `TryParse` | `true` | `TryParse(object?)` overloads |
| `CaseInsensitiveEquals` | `true` | `Equals()` + `GetHashCode()` case-insensitive |
| `UseInvariantCulture` | `false` | `InvariantCultureIgnoreCase` instead of `OrdinalIgnoreCase` |

Always generated: `implicit operator string`, `ToString()`.

**Extracting the primitive value**: use implicit conversion (`string id = personId;`), `ToString()`, or `int.Parse(vo.ToString())` for numeric types.

### Manual (BC SharedKernel — BC-specific types)
Same pattern but located in `{BC}.SharedKernel`. Can also use `[ValueObject]`.

### Shared Records
Simple shared records (e.g., `Device(string Name, string Key)`) go in `SharedKernel/Shared/`.

---

## Contract Versioning

- Contracts: `Contracts/{BC}/{Feature}/{Action}/v1/`
- Handlers mirror: `Application/{BC}/UseCases/{Action}/v1/`
- Namespace follows folder. The Contracts **project** name depends on whether the BC is still part of the monolith or has been promoted to its own microservice:
  - **Default (monolith)**: a single Contracts project per subdomain — `{SubDomain}.Contracts` — holding every BC. Namespace: `{SubDomain}.Contracts.{BC}.{Feature}.{Action}.v1` (e.g., `IdentityTrix.Contracts.Persons.Cellphones.AddCellphone.v1`).
  - **BC promoted to microservice**: that BC gets its own Contracts project — `{SubDomain}.{BC}.Contracts`. Namespace: `{SubDomain}.{BC}.Contracts.{Feature}.{Action}.v1` (e.g., `IdentityTrix.Persons.Contracts.Cellphones.AddCellphone.v1`).
- Similar commands can share a base validator; shared internal DTOs go in `SharedData/` and end with `Data`

---

## Allowed Packages

NEVER add packages. Ask the developer first if you need something new.

NEVER use FluentAssertions directly. Always use base methods.
NEVER use test packages except these:
- Testcontainers.*
- Moq
- AutoFixture

---

## Code Scaffolds

### Command

**Contract** (`{BC}.Contracts/{Feature}/{Action}/v1/`):
```csharp
public record AddCellphoneToPersonCommand : IAxisCommand<AddCellphoneToPersonResponse>
{
    public string? PersonId { get; init; }
    public string? CountryId { get; init; }
    public string? CellphoneNumber { get; init; }
}

public record AddCellphoneToPersonResponse : IAxisCommandResponse
{
    public required CellphoneId CellphoneId { get; init; }
}
```

**Handler** (`{BC}.Application/{Feature}/UseCases/{Action}/v1/`):
```csharp
internal class AddCellphoneToPersonHandler(
    IPersonAggregateApplicationFactory personApplicationFactory,
    ICellphonesMediator cellphoneMediator,
    IUnitOfWorkProvider unitOfWorkProvider
) : ICommandHandler<AddCellphoneToPersonCommand, AddCellphoneToPersonResponse>
{
    public Task<AxisResult<AddCellphoneToPersonResponse>> HandleAsync(AddCellphoneToPersonCommand cmd)
        => personApplicationFactory.GetByIdAsync(cmd.PersonId)
            .ThenAsync(application => cellphoneMediator.AddAsync(new() { CountryId = cmd.CountryId, CellphoneNumber = cmd.CellphoneNumber })
                .TapAsync(r => application.AddCellphoneAsync(r.CellphoneId))
                .TapAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
                .MapAsync(r => new AddCellphoneToPersonResponse { CellphoneId = r.CellphoneId }));
}
```

**Validator**:
```csharp
internal class AddCellphoneToPersonValidator : AxisValidatorBase<AddCellphoneToPersonCommand>
{
    public AddCellphoneToPersonValidator()
    {
        RequiredTryParse(x => x.CountryId, "COUNTRY_ID_NULL_OR_NOT_VALID", value => CountryId.TryParse(value?.ToString(), out _));
        NotNullOrEmpty(x => CountryIds.GetById(x.CountryId), "COUNTRY_ID_NULL_OR_NOT_VALID",
            countryId => RequiredCellPhone(x => x.CellphoneNumber, countryId!.Value, "CELLPHONE_NUMBER_NULL_OR_NOT_VALID"));
    }
}
```

After creating a command, add it to the corresponding SDK mediator interface and implementation. If no mediator exists, create one.

---

### Query

```csharp
public record GetPersonByEmailQuery : IAxisQuery<GetPersonByEmailResponse>
{
    public string? Email { get; init; }
}

public record GetPersonByEmailResponse : IAxisQueryResponse
{
    public required PersonId PersonId { get; init; }
    public required string DisplayName { get; init; }
    public string? PictureProxyUrl { get; init; }
}
```

```csharp
internal class GetPersonByEmailHandler(
    IPersonsReaderPort readerPort,
    IEmailsMediator emailsMediator
) : IQueryHandler<GetPersonByEmailQuery, GetPersonByEmailResponse>
{
    public Task<AxisResult<GetPersonByEmailResponse>> HandleAsync(GetPersonByEmailQuery query)
        => emailsMediator.GetByAddressAsync(new() { Email = query.Email })
            .ThenAsync(r => readerPort.GetByEmailAsync(r.EmailId))
            .MapAsync(entity => new GetPersonByEmailResponse
            {
                DisplayName = entity.DisplayName,
                PersonId = entity.PersonId,
                PictureProxyUrl = entity.PictureProxyUrl
            });
}
```

Queries do NOT call `SaveChangesAsync()`. Queries go directly handler → port, never through Application/Domain.

---

### Entity

**SharedKernel interface**:
```csharp
public interface IPersonEntityProperties
{
    bool Active { get; }
    PersonId PersonId { get; }
    string DisplayName { get; }
    string? PictureProxyUrl { get; }
    string OriginId { get; }
}
```

**Properties** (`{Aggregate}/Root/{Entity}EntityProperties.cs`):
```csharp
internal partial class PersonEntity(
    bool active, PersonId personId, string displayName, string? pictureProxyUrl, string originId
) : IPersonEntityProperties
{
    public bool Active { get; } = active;
    public PersonId PersonId { get; } = personId;
    public string DisplayName { get; } = displayName;
    public string? PictureProxyUrl { get; } = pictureProxyUrl;
    public string OriginId { get; } = originId;

    internal PersonEntity(IPersonEntityProperties p) : this(p.Active, p.PersonId, p.DisplayName, p.PictureProxyUrl, p.OriginId) {}
}
```

**Rules** (`{Entity}EntityRules.cs`):
```csharp
internal partial class PersonEntity
{
    private AxisResult CheckIfIsActive()
        => !Active ? AxisResult.BusinessRule("PERSON_NOT_ACTIVE") : AxisResult.Ok();

    public AxisResult AddCellphone() => CheckIfIsActive();
    public AxisResult AddEmail() => CheckIfIsActive();
}
```

---

### Repository

**Table constants**:
```csharp
internal static class ExternalApisTable
{
    public const string Table = $"{DatabaseScripts.Schema}.EXTERNAL_APIS";
    public const string ExternalApiId = "EXTERNAL_API_ID";
    public const string Name = "NAME";
    public const string Secret = "SECRET";

    public const string V1 = $"""
        CREATE TABLE IF NOT EXISTS {Table}
        (
            {ExternalApiId} UUID PRIMARY KEY,
            {Name} VARCHAR(250) NOT NULL,
            {Secret} VARCHAR(500) NOT NULL
        );
    """;
}
```

**DbEntity**:
```csharp
internal record ExternalApiDbEntity(ExternalApiId ExternalApiId, string ApiName, string HashedSecret)
    : IExternalApiEntityProperties
{
    internal static ExternalApiDbEntity FromReader(NpgsqlDataReader reader) => new(
        ExternalApiId: reader.GetGuid(0).ToString(),
        ApiName: reader.GetString(1),
        HashedSecret: reader.GetString(2)
    );
}
```

**Repository**:
```csharp
internal class ExternalApisRepository(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    [FromKeyedServices(ApplicationConfig.AppKey)] IPostgresUnitOfWork uow
) : PostgresRepositoryBase(mediator, logger, uow), IExternalApisReaderPort, IExternalApisWritePort
{
    private const string Select = $"SELECT {ExternalApisTable.ExternalApiId}, {ExternalApisTable.Name}, {ExternalApisTable.Secret}";

    public Task<AxisResult> CreateAsync(IExternalApiEntityProperties properties)
        => ExecuteAsync(
            $"INSERT INTO {ExternalApisTable.Table} ({ExternalApisTable.ExternalApiId}, {ExternalApisTable.Name}, {ExternalApisTable.Secret}) VALUES (@id, @name, @secret)",
            p =>
            {
                p.AddWithValue("id", Guid.Parse(properties.ExternalApiId.ToString()));
                p.AddWithValue("name", properties.ApiName);
                p.AddWithValue("secret", properties.HashedSecret);
            },
            duplicateKeyCode: "EXTERNAL_API_ALREADY_EXISTS");

    public Task<AxisResult<IExternalApiEntityProperties>> GetByIdAsync(ExternalApiId id)
        => GetAsync<IExternalApiEntityProperties>(
            $"{Select} FROM {ExternalApisTable.Table} WHERE {ExternalApisTable.ExternalApiId} = @id",
            p => p.AddWithValue("id", id.ToString()),
            ExternalApiDbEntity.FromReader,
            "EXTERNAL_API_NOT_FOUND");

    public Task<AxisResult> UpdateSecretAsync(ExternalApiId id, string hashedSecret)
        => ExecuteAsync(
            $"UPDATE {ExternalApisTable.Table} SET {ExternalApisTable.Secret} = @secret WHERE {ExternalApisTable.ExternalApiId} = @id",
            p =>
            {
                p.AddWithValue("id", id.ToString());
                p.AddWithValue("secret", hashedSecret);
            });
}
```

**DatabaseScripts + Migrations**:
```csharp
public static class DatabaseScripts
{
    public const string Schema = "IDENTITY_TRIX_PERSONS";
    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", PersonsTable.V1 + PersonCellphonesTable.V1),
        ("V2", "ALTER TABLE ..."),
    ];
}

public static class IdentityTrixPersonsMigrations
{
    private const string MigrationsTable = $"{DatabaseScripts.Schema}.SCHEMA_MIGRATIONS";

    public static async Task InitializePostgresAsync(string connectionString)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        // create schema + migrations table, then apply each version idempotently in a transaction
    }
}
```

---

### Application Layer

**Factory** (`{Aggregate}AggregateApplicationFactory.cs` — interface + impl in same file):
```csharp
internal interface IPersonAggregateApplicationFactory
{
    Task<AxisResult<IPersonAggregateApplication>> GetByIdAsync(PersonId personId);
    Task<AxisResult<IPersonAggregateApplication>> CreateAsync(NewArgs args);

    public record NewArgs
    {
        public required NationalId NationalId { get; init; }
        public required string DisplayName { get; init; }
        public string? PictureProxyUrl { get; init; }
        public required string OriginId { get; init; }
    }
}

internal class PersonAggregateApplicationFactory(
    IAxisMediator mediator,
    IPersonsReaderPort readerPort,
    IPersonsWritePort writePort,
    IPersonCellphonesWriter cellphonesWriter,
    IPersonEmailsWriter emailsWriter
) : IPersonAggregateApplicationFactory
{
    private IPersonAggregateApplication NewInstance(IPersonEntityProperties p)
        => new PersonAggregateApplication(p, cellphonesWriter, emailsWriter);

    public Task<AxisResult<IPersonAggregateApplication>> GetByIdAsync(PersonId personId)
        => readerPort.GetByIdAsync(personId).MapAsync(NewInstance);

    public Task<AxisResult<IPersonAggregateApplication>> CreateAsync(IPersonAggregateApplicationFactory.NewArgs args)
        => readerPort.GetByNationalIdAsync(args.NationalId)
            .RequireNotFound(AxisError.BusinessRule("DOCUMENT_ID_ALREADY_ADDED"))
            .ThenAsync(() =>
            {
                IPersonEntityProperties newEntity = new PersonEntity(true, PersonId.New, args.DisplayName, args.PictureProxyUrl, args.OriginId);
                return AxisResult.Ok(newEntity).TapAsync(writePort.CreateAsync);
            })
            .MapAsync(NewInstance);
}
```

**AggregateApplication** (`{Aggregate}AggregateApplication.cs` — interface + impl in same file):
```csharp
internal interface IPersonAggregateApplication : IPersonEntityProperties
{
    Task<AxisResult> AddCellphoneAsync(CellphoneId cellphoneId);
    Task<AxisResult> AddEmailAsync(EmailId emailId);
}

internal class PersonAggregateApplication(
    IPersonEntityProperties properties,
    IPersonCellphonesWriter cellphonesWriter,
    IPersonEmailsWriter emailsWriter
) : PersonEntity(properties), IPersonAggregateApplication
{
    public Task<AxisResult> AddCellphoneAsync(CellphoneId cellphoneId)
        => AddCellphone().TapAsync(() => cellphonesWriter.AddCellphoneAsync(PersonId, cellphoneId));

    public Task<AxisResult> AddEmailAsync(EmailId emailId)
        => AddEmail().TapAsync(() => emailsWriter.AddEmailAsync(PersonId, emailId));
}
```

---

### SDK

**Interface** (`{BC}.SDK/{Feature}/v1/`):
```csharp
public interface IPersonsMediator
{
    Task<AxisResult<AddCellphoneToPersonResponse>> AddCellphoneAsync(AddCellphoneToPersonCommand command);
    Task<AxisResult<GetPersonByEmailResponse>> GetByEmailAsync(GetPersonByEmailQuery query);
}
```

**Implementation** (`{BC}.SDK.Application/{Feature}/v1/`):
```csharp
internal class PersonsMediator(IAxisMediator mediator) : IPersonsMediator
{
    public Task<AxisResult<AddCellphoneToPersonResponse>> AddCellphoneAsync(AddCellphoneToPersonCommand command)
        => mediator.Cqrs.ExecuteAsync<AddCellphoneToPersonCommand, AddCellphoneToPersonResponse>(command);

    public Task<AxisResult<GetPersonByEmailResponse>> GetByEmailAsync(GetPersonByEmailQuery query)
        => mediator.Cqrs.QueryAsync<GetPersonByEmailQuery, GetPersonByEmailResponse>(query);
}
```

**DI** (`{BC}.SDK.Application/DependencyInjection.cs`):
```csharp
public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddIdentityTrixPersonsSdkApplication(this ServiceCollectionBuilder builder)
    {
        builder.Services.AddScoped<IPersonsMediator, PersonsMediator>();
        return builder.AddPersonsApplication()
            .AddCqrsMediator(Assembly.GetExecutingAssembly());
    }
}
```

---

### Unit Tests

```csharp
// BaseUnitTest.cs
public class BaseUnitTest
{
    protected static IServiceProvider DefaultServiceProvider()
    {
        var sp = new ServiceCollection()
            .InitAxisTrixAdd()
            .AddMocks()
            .AddMySdkApplication()
            .EndAxisTrixAdd_Mock()
            .BuildServiceProvider();

        var ctx = sp.GetRequiredService<IMediatorContextAccessor>();
        ctx.OriginId = $"ExternalApiTrix-{Guid.NewGuid():N}";
        ctx.PersonId = Guid.CreateVersion7().ToString();
        return sp;
    }

    protected static IServiceProvider SetMockServices(IServiceCollection services)
        => services.InitAxisTrixAdd().AddMySdkApplication().EndAxisTrixAdd_Mock().BuildServiceProvider();
}
```

- **Handler tests**: Use `SetMockServices()` with Moq — real CQRS pipeline runs (validator + handler)
- **Validation tests**: Use `DefaultServiceProvider()` with default mocks — only verify rejection
- Resolve SDK mediator (not handler directly) to test the full pipeline
- Assert: `Assert.True(result.IsSuccess)`, `Assert.True(result.IsFailure)`, `Assert.Contains(result.Errors, x => x.Code == "CODE")`
- Use private `MockProperties` classes implementing `I{Entity}EntityProperties`
- NEVER use FluentAssertions

---

### Integration Tests

```csharp
// PostgresFixture.cs
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("test").WithUsername("admin").WithPassword("password")
            .WithCleanUp(true).Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await RunMigrationsAsync(ConnectionString);
    }

    public static async Task RunMigrationsAsync(string cs)
    {
        await DataPrivacyTrixMigrations.InitializePostgresAsync(cs);
        await IdentityTrixPersonsMigrations.InitializePostgresAsync(cs);
        // add more if cross-domain dependencies
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();
}

[CollectionDefinition("PostgresCollection")]
public class PostgresCollection : ICollectionFixture<PostgresFixture>;

[Collection("PostgresCollection")]
public abstract class DatabaseTestBase(PostgresFixture fixture)
{
    protected readonly PostgresFixture Fixture = fixture;
}
```

```csharp
// DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceProvider ServiceProviderWithPostgres(string connectionString)
    {
        var sp = new ServiceCollection()
            .InitAxisTrixAdd()
            .AddMemoryCacheTrix()
            .AddDataPrivacyTrixPostgres(connectionString)
            .AddDataPrivacyTrixSDKApplication()
            .AddIdentityTrixPersonsPostgres(connectionString)
            .AddIdentityTrixPersonsSdkApplication()
            .EndAxisTrixAdd_Mock()
            .BuildServiceProvider();

        var ctx = sp.GetRequiredService<IMediatorContextAccessor>();
        ctx.OriginId = $"ExternalApiTrix-{Guid.NewGuid():N}";
        ctx.PersonId = Guid.CreateVersion7().ToString();
        return sp;
    }
}
```

- Use real PostgreSQL container (Testcontainers), not mocks
- `[CollectionDefinition]` + `ICollectionFixture<>` shares container across tests
- Each test operation uses `using var scope = serviceProvider.CreateScope()`
- Always include a migration idempotency test: call `InitializePostgresAsync()` twice
- Cross-domain tests: include all dependency migrations and DI registrations
- Assert failure messages with error codes: `$"Failed: {string.Join("; ", result.Errors.Select(e => e.Code))}"`

---

## DI Composition Example

```
SDK.Application (entry point)
├── AddIdentityTrixPersonsSdkApplication()
│   ├── IPersonsMediator → PersonsMediator (Scoped)
│   └── AddPersonsApplication()
│       ├── AddPersonsModule() [internal]
│       │   └── IPersonAggregateApplicationFactory → PersonAggregateApplicationFactory (Scoped)
│       └── AddCqrsMediator() — discovers handlers + validators

Repository.Postgres
└── AddIdentityTrixPersonsPostgres(connectionString)
    ├── AddPostgresUnitOfWork(appKey, connectionString)
    ├── IUnitOfWorkProvider → UnitOfWorkProvider (Scoped)
    └── AddPersonRepository() [internal]
        ├── IPersonsReaderPort → PersonsRepository (Scoped)
        └── IPersonsWritePort → PersonsRepository (Scoped)
```

**Cross-subdomain**: Authentication → Persons (via `IdentityTrix.Persons.SDK`). One-way dependency — Persons does NOT reference Authentication.

---

## Reference

Error Code Prefixes: `PERSON_`, `AUTH_CODE_`, `EMAIL_`, `CELLPHONE_`, `TOKEN_`, `DOCUMENT_ID_`, `ORIGIN_ID_`, `DEVICE_`, `NATIONAL_ID_`

Common suffixes: `_NULL_OR_NOT_VALID`, `_NULL_OR_EMPTY`, `_NOT_FOUND`, `_ALREADY_ADDED`, `_NOT_ACTIVE`, `_ALREADY_VALIDATED`
