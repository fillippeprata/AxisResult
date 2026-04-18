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
    │   ├── {BC}/v1/{Feature}/{UseCase}/I{Feature}Mediator       # Interface for SDK Implementation
    │   └── {BC}/v1/{UseCase}/                                   # If the feature has the same name as the BC (CRUDs), there is no feature folder
    ├── Core/
    │   ├── {SubDomain}.SharedKernel/                            # Entity interfaces, Value Objects, ApplicationConfig
    │   ├── {SubDomain}.Domain/                                  # Entities (Properties + Rules, partial classes)
    │   └── {SubDomain}.Application/                             # Handlers, Validators, Factories, AggregateApplications
    │       ├── {BC}/                                            # DependencyInjection, AggregateApplicationFactory, AggregateApplications
    │       └── {BC}/UseCases/v1/{SameContractStructure}/        # Handlers and Validators
    ├── Ports/{SubDomain}.Ports/                                 # Reader/Writer port interfaces, IUnitOfWorkProvider
    ├── Adapters/
    │   ├── Driven/
    │   │   ├── Repositories/{SubDomain}.Driven.Repositories.[Technology]/  # Postgres | MongoDb | etc.
    │   │   ├── Producers/{SubDomain}.Driven.Producers.[Technology]/        # Kafka | RabbitMq | etc.
    │   │   ├── Consumers/{SubDomain}.Driven.Consumers.[Technology]/        # Kafka | RabbitMq | etc.
    │   │   └── Others/
    │   └── Driving/
    │       ├── {SubDomain}.Sdk.Application/                     # SDK for Dependency Injection
    │       ├── {SubDomain}.Sdk.HttpClient/                      # SDK for HTTP Communication
    │       ├── {SubDomain}.Sdk.GrpcClient/                      # SDK for gRPC Communication
    │       ├── {SubDomain}.WebApi/                               # Web Api for microservices
    │       └── {SubDomain}.Grpc/                                 # gRPC for microservices
    └── Tests/
        ├── {SubDomain}.UnitTests/
        └── {SubDomain}.IntegrationTests/
```

If the architecture is not microservices-based, there will be no integration with Web API or gRPC. The monolith will use the SDK.Application to execute the use cases.

**Project naming**: `{SubDomain}.{Layer}` — e.g., `IdentityTrix.Application`, `IdentityTrix.Driven.Repositories.Postgres`, `IdentityTrix.Sdk.Application`. Driven/Driving adapters include the adapter direction prefix in the project name.

### Foundation Modules

The Foundation layer is split into independent modules:

| Module | Project / Package | Purpose |
|--------|-------------------|---------|
| **Results** | `AxisResult` (NuGet: https://www.nuget.org/packages/AxisResult) | `AxisResult`, `AxisError`, `AxisErrorType` — external NuGet package. Domain projects that only need `AxisResult` (e.g., for entity rules) should reference this package directly instead of the full Foundation |
| **Types** | `AxisTrix.Types` | Shared types (`CountryId`, `PersonData`) — namespaced under `AxisTrix.Types.Localization`, `AxisTrix.Types.Persons` |
| **Foundation** | `AxisTrix.Foundation` | CQRS, Validation, Pipelines, Logging, Telemetry — references the `AxisResult` NuGet package + Types |
| **SourceGen** | `AxisTrix.SourceGen` | `[ValueObject]` source generator |

> **AxisResult is no longer an in-repo project.** It has been extracted to the public NuGet package [`AxisResult`](https://www.nuget.org/packages/AxisResult). Add it via `<PackageReference Include="AxisResult" />` (version managed in `Directory.Packages.props`). Never recreate a local `AxisTrix.Results` / `AxisResult` project inside this repo.

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
- Use `.ThenAsync()` (not `.TapAsync()`) for failable operations whose errors must propagate (UoW, write ports, cache invalidation). Use `.TapAsync()` only for fire-and-forget side effects (logging, metrics)

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
| `.ActionAsync(func)` | Run a **failable async side effect** (`Func<TValue, Task<AxisResult>>`) that **preserves the original value** — use for domain validation in factories (e.g., `.ActionAsync(app => app.IsValidAsync())`) |
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
| `.RequireNotFoundAsync(error)` | Async extension (Task/ValueTask) version of `RequireNotFound` — works on both `Task<AxisResult>` and `Task<AxisResult<TValue>>`, always returns `AxisResult` (untyped) |

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
> 
> **C# 14 extension members**: `AxisResultExtensions.cs` uses the new `extension(Type) { ... }` syntax (C# 14 preview). This replaces the traditional `static class` + `this` parameter style for extension methods.                            

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
| `DependentRules` | `<TProperty1, TProperty2>(expression1, errorCode1, expression2, errorCode2, Func<TProperty1, TProperty2, AxisResult> dependentRules)` | Validates two properties are not null, then runs a cross-field validation function returning `AxisResult` |

#### Dependent Rules Pattern

There are two patterns for cross-field validation:

**Pattern 1 — `DependentRules<TProperty1, TProperty2>`** (preferred for two-property cross-validation with domain logic):

```csharp
// Validate CountryId and CellphoneNumber are not null, then run domain validation (returns AxisResult)
DependentRules<CountryId, string>(
    x => (CountryId)x.CountryId,
    "COUNTRY_ID_NULL_OR_NOT_VALID",
    x => x.CellphoneNumber,
    "CELLPHONE_NUMBER_NULL_OR_NOT_VALID",
    (countryId, cellphone) => countryId.GetFormattedPhone(cellphone)
);
```

**How it works**: validates both expressions are not null (short-circuits on first failure), then invokes the `Func<TProperty1, TProperty2, AxisResult>` with both validated values. If the function returns a failure `AxisResult`, the second property's error code is used.

**Pattern 2 — `NotNullOrEmpty` with `Action<TProperty>`** (for chaining one property into another validator):

```csharp
// Validate CountryId via TryParse first, then chain document validation using a selector
RequiredTryParse(x => x.CountryId, "COUNTRY_ID_NULL_OR_NOT_VALID", value => CountryId.TryParse(value?.ToString(), out _));
DocumentId(x => x.DocumentId, instance => instance.CountryId, "DOCUMENT_ID_NULL_OR_NOT_VALID");
```

**How it works**: `NotNullOrEmpty(expr, errorCode, Action<TProperty>)` first validates the expression is not null. Only if it passes, it invokes the lambda with the **already-validated non-null value**, allowing you to safely use it as input for the next validation rule.

---

## Domain Entities

Each entity has a Properties file; a Rules file is only needed when the entity has domain rules:

**`{Entity}EntityProperties.cs`** — `internal partial class` with primary constructor, implements `I{Entity}EntityProperties`, immutable `{ get; }` properties. Secondary constructor accepts the interface for rehydration. If the entity has no domain rules, this is the only file and the class does NOT need to be `partial`.

**`{Entity}EntityRules.cs`** — `internal partial class`, business methods returning `AxisResult` or `Task<AxisResult>`. Private methods for internal rules; public/protected methods compose private ones. Static methods for value generation. Rules that delegate to domain-specific validation (e.g., phone formatting) should use `protected` access so the AggregateApplication can expose them via the `new` keyword.

**SharedKernel interface** — `I{Entity}EntityProperties` defines the read contract. Can have computed properties (e.g., `bool IsActive => WasAuthenticated && !CanceledAt.HasValue`).

**Domain-specific validation** — Country-specific validators (e.g., phone formatting, document validation) live in the Domain layer under `{Aggregate}/Validation/`, NOT in Foundation. They return `AxisResult<T>` and are called from Entity Rules.

Root entities: `{Aggregate}/Root/`. Non-root: `{Aggregate}/Entities/`.

---

## Application Layer (Factory + AggregateApplication)

**Core rules**:
- Domain Entity NEVER accessed directly — Application inherits from Entity and is exposed via interface
- Application Factory receives application injections (Ports/AxisMediator)

**Factory** (`I{Name}AggregateApplicationFactory`):
- Methods: `GetByIdAsync()` and `CreateAsync(NewArgs)` where `NewArgs` is a nested record
- `GetByIdAsync()` → reader port + `.MapAsync()` to wrap in Application, optionally + `.ActionAsync()` for domain validation
- `CreateAsync()` → use `.RequireNotFoundAsync()` for duplicate detection, then `.ThenAsync(writePort.CreateAsync)`
- When the entity has domain validation rules, use `.ActionAsync(app => app.IsValidAsync())` after `.MapAsync(NewInstance)` to validate before returning
- `NewInstance(I{Entity}EntityProperties properties)` — **private** helper that constructs the Application by passing `properties` directly to the AggregateApplication constructor (never wrap in `new {Entity}Entity(properties)` first — the AggregateApplication's base call handles the rehydration). Use `private static` when the AggregateApplication does not need injected dependencies; use `private` (non-static) when the factory needs to pass ports or services to the Application constructor

**AggregateApplication** (`I{Name}AggregateApplication : I{Entity}EntityProperties`):
- `internal class {Name}AggregateApplication(I{Entity}EntityProperties properties, ...) : {Entity}(properties), I{Name}AggregateApplication`
- The first constructor parameter is always `I{Entity}EntityProperties properties` — never the concrete `{Entity}Entity` type. The base call `: {Entity}(properties)` rehydrates the entity from the interface via its secondary constructor.
- Coordinates: call the inherited domain rule directly (no `root.` prefix) → `.TapAsync(() => {child}Writer.{Action}())`. Example: `AddCellphone().TapAsync(() => cellphonesWriter.AddCellphoneAsync(PersonId, cellphoneId))`.
- When the entity has `protected` domain rules, expose them via `public new` methods: `public new Task<AxisResult> IsValidAsync() => base.IsValidAsync();`

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

**`{Aggregate}DbInit`** — `public static class` per aggregate (e.g., `ExternalApisDbInit`) with `const string Schema` and `internal static readonly (string Version, string Script)[] Migrations`. Each aggregate owns its schema and migrations, enabling future extraction to microservices.

**Table DDL** — each Table class defines `const string V1` (and `V2`, `V3` for incremental changes). References `{Aggregate}DbInit.Schema` for the schema prefix.

**Migration runner** — `{Aggregate}Migrations` (internal) applies migrations per aggregate. A public facade `{SubDomain}Migrations.InitializePostgresAsync(string connectionString)` orchestrates all aggregate migrations:
- Creates schema + `SCHEMA_MIGRATIONS` table (`VERSION VARCHAR(50) PK`, `APPLIED_AT TIMESTAMPTZ`)
- Applies migrations idempotently in a transaction

---

## SDK / Driving Adapters

- Public interface: `I{Name}Mediator` in `{Subdomain}.Contracts`
- Internal DI implementation: `{Name}Mediator` in `{Subdomain}.Sdk.Application`
- Primary constructor: `IAxisMediator mediator`
- Commands: `mediator.Cqrs.ExecuteAsync<TCommand, TResponse>(command)`
- Queries: `mediator.Cqrs.QueryAsync<TQuery, TResponse>(query)`
- SDK is a thin wrapper — zero logic, only delegates to CQRS pipeline
- Sdk.Application DI calls `Add{Subdomain}Application()` — it is the entry point that wires everything

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

- Contracts: `Contracts/{BC}/v1/{Action}/` (or `Contracts/{BC}/v1/{Feature}/{Action}/` when the feature differs from the BC)
- Handlers mirror: `Application/{BC}/UseCases/v1/{Action}/`
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

**Contract** (`{BC}.Contracts/{Feature}/v1/{Action}/`):
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

**Handler** (`{BC}.Application/{Feature}/UseCases/v1/{Action}/`):
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
                .ThenAsync(r => application.AddCellphoneAsync(r.CellphoneId))
                .ThenAsync(_ => unitOfWorkProvider.UnitOfWork.SaveChangesAsync())
                .MapAsync(r => new AddCellphoneToPersonResponse { CellphoneId = r.CellphoneId }));
}
```

**Validator**:
```csharp
internal class AddCellphoneToPersonValidator : AxisValidatorBase<AddCellphoneToPersonCommand>
{
    public AddCellphoneToPersonValidator()
    {
        RequiredTryParse(x => x.PersonId, "PERSON_ID_NULL_OR_NOT_VALID", value => PersonId.TryParse(value?.ToString(), out _));
        RequiredTryParse(x => x.CountryId, "COUNTRY_ID_NULL_OR_NOT_VALID", value => CountryId.TryParse(value?.ToString(), out _));
        NotNullOrEmpty(x => x.CellphoneNumber, "CELLPHONE_NUMBER_NULL_OR_NOT_VALID");
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

**Rules with domain validation** (e.g., `CellphoneEntityRules.cs`):
```csharp
internal partial class CellphoneEntity
{
    protected Task<AxisResult> IsValidAsync()
        => Task.FromResult<AxisResult>(CountryId.GetFormattedPhone(CellphoneNumber));
}
```
Use `protected` when the rule needs to be exposed through the AggregateApplication via `public new`.

---

### Repository

**Table constants**:
```csharp
internal static class ExternalApisTable
{
    public const string Table = $"{ExternalApisDbInit.Schema}.EXTERNAL_APIS";
    public const string ExternalApiId = "EXTERNAL_API_ID";
    public const string Name = "NAME";
    public const string Secret = "HASHED_SECRET";

    public const string V1 = $"""
        CREATE TABLE IF NOT EXISTS {Table}
        (
            {ExternalApiId} VARCHAR(250) PRIMARY KEY,
            {Name} VARCHAR(250) NOT NULL UNIQUE,
            {Secret} VARCHAR(512) NOT NULL
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

**DbInit + Migrations** (per aggregate):
```csharp
// {Aggregate}DbInit.cs — owns schema and migration definitions for this aggregate
public static class ExternalApisDbInit
{
    public const string Schema = "IDENTITY_TRIX_EXTERNAL_APIS";

    internal static readonly (string Version, string Script)[] Migrations =
    [
        ("V1", ExternalApisTable.V1),
        ("V2", V2),
    ];

    private static string V2 => $"""
        INSERT INTO {ExternalApisTable.Table} (...)
        SELECT ... WHERE NOT EXISTS (...);
        """;
}

// {Aggregate}Migrations.cs — internal runner for this aggregate
internal static class ExternalApisMigrations
{
    private const string MigrationsTable = $"{ExternalApisDbInit.Schema}.SCHEMA_MIGRATIONS";

    public static async Task InitializePostgresAsync(string connectionString)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        // create schema + migrations table, then apply each version idempotently in a transaction
    }
}

// {SubDomain}Migrations.cs — public facade that orchestrates all aggregate migrations
public static class IdentityTrixMigrations
{
    public static Task InitializePostgresAsync(string connectionString)
        => ExternalApisMigrations.InitializePostgresAsync(connectionString);
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
            .RequireNotFoundAsync(AxisError.BusinessRule("DOCUMENT_ID_ALREADY_ADDED"))
            .WithValueAsync(new PersonEntity(true, PersonId.New, args.DisplayName, args.PictureProxyUrl, args.OriginId))
            .MapAsync(NewInstance)
            .ThenAsync(writePort.CreateAsync);
}
```

**Factory with domain validation** (e.g., `CellphoneAggregateApplicationFactory.cs`):
```csharp
internal class CellphoneAggregateApplicationFactory(
    ICellphonesReaderPort readerPort,
    ICellphonesWritePort writePort
) : ICellphoneAggregateApplicationFactory
{
    private static ICellphoneAggregateApplication NewInstance(ICellphoneEntityProperties properties)
        => new CellphoneAggregateApplication(properties);

    public Task<AxisResult<ICellphoneAggregateApplication>> GetByIdAsync(CellphoneId id)
        => readerPort.GetByIdAsync(id)
            .MapAsync(NewInstance)
            .ActionAsync(app => app.IsValidAsync());  // Domain validation after load

    public Task<AxisResult<ICellphoneAggregateApplication>> CreateAsync(NewArgs args)
        => GetByCellphoneNumberAsync(args.CountryId, args.CellphoneNumber)
            .RequireNotFoundAsync(AxisError.ValidationRule("CELLPHONE_ALREADY_EXISTS"))
            .WithValueAsync(new CellphoneEntity(CellphoneId.New, args.CountryId, args.CellphoneNumber))
            .MapAsync(NewInstance)
            .ActionAsync(app => app.IsValidAsync())    // Domain validation before save
            .ThenAsync(writePort.CreateAsync);
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

**AggregateApplication exposing protected entity rules** (e.g., `CellphoneAggregateApplication.cs`):
```csharp
internal interface ICellphoneAggregateApplication : ICellphoneEntityProperties
{
    Task<AxisResult> IsValidAsync();
}

internal class CellphoneAggregateApplication(
    ICellphoneEntityProperties properties
) : CellphoneEntity(properties), ICellphoneAggregateApplication
{
    public new Task<AxisResult> IsValidAsync()
        => base.IsValidAsync();
}
```

---

### SDK

**Interface** (`{BC}.Contracts/{Feature}/v1/`):
```csharp
public interface IPersonsMediator
{
    Task<AxisResult<AddCellphoneToPersonResponse>> AddCellphoneAsync(AddCellphoneToPersonCommand command);
    Task<AxisResult<GetPersonByEmailResponse>> GetByEmailAsync(GetPersonByEmailQuery query);
}
```

**Implementation** (`{BC}.Sdk.Application/{Feature}/v1/`):
```csharp
internal class PersonsMediator(IAxisMediator mediator) : IPersonsMediator
{
    public Task<AxisResult<AddCellphoneToPersonResponse>> AddCellphoneAsync(AddCellphoneToPersonCommand command)
        => mediator.Cqrs.ExecuteAsync<AddCellphoneToPersonCommand, AddCellphoneToPersonResponse>(command);

    public Task<AxisResult<GetPersonByEmailResponse>> GetByEmailAsync(GetPersonByEmailQuery query)
        => mediator.Cqrs.QueryAsync<GetPersonByEmailQuery, GetPersonByEmailResponse>(query);
}
```

**DI** (`{BC}.Sdk.Application/DependencyInjection.cs`):
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
            .AddDataPrivacyTrixSdkApplication()
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
Sdk.Application (entry point)
├── AddIdentityTrixPersonsSdkApplication()
│   ├── IPersonsMediator → PersonsMediator (Scoped)
│   └── AddPersonsApplication()
│       ├── AddPersonsModule() [internal]
│       │   └── IPersonAggregateApplicationFactory → PersonAggregateApplicationFactory (Scoped)
│       └── AddCqrsMediator() — discovers handlers + validators

Driven.Repositories.Postgres
└── AddIdentityTrixPersonsPostgres(connectionString)
    ├── AddPostgresUnitOfWork(appKey, connectionString)
    ├── IUnitOfWorkProvider → UnitOfWorkProvider (Scoped)
    └── AddPersonRepository() [internal]
        ├── IPersonsReaderPort → PersonsRepository (Scoped)
        └── IPersonsWritePort → PersonsRepository (Scoped)
```

**Cross-subdomain**: Authentication → Persons (via `IdentityTrix.Persons.Sdk`). One-way dependency — Persons does NOT reference Authentication.

---

## Reference

Error Code Prefixes: `PERSON_`, `AUTH_CODE_`, `EMAIL_`, `CELLPHONE_`, `TOKEN_`, `DOCUMENT_ID_`, `ORIGIN_ID_`, `DEVICE_`, `NATIONAL_ID_`

Common suffixes: `_NULL_OR_NOT_VALID`, `_NULL_OR_EMPTY`, `_NOT_FOUND`, `_ALREADY_ADDED`, `_NOT_ACTIVE`, `_ALREADY_VALIDATED`
