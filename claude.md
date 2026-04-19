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
- **Skip task tracking for linear mechanical changes**: For isolated, single-vertical modifications that follow a known checklist (e.g., adding/removing a property on an aggregate, renaming a use case), do NOT use TaskCreate. The overhead of tracking exceeds the benefit. Use TaskCreate only when work has true branches, parallel paths, or discovery steps that may reveal new subtasks.

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
    │       └── {BC}/UseCases/{SameContractStructure}/v1/        # Handlers and Validators
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

**Project naming**: `{SubDomain}.{Layer}` — e.g., `TenantTrix.Application`, `TenantTrix.Driven.Repositories.Postgres`, `TenantTrix.Sdk.Application`. Driven/Driving adapters include the adapter direction prefix in the project name.

**Foundation**: `AxisResult` is an external NuGet package (see [`AxisResult.md`](AxisResult.md) — never recreate locally). `AxisTrix.Types`, `AxisTrix.Foundation`, and `AxisTrix.SourceGen` (`[ValueObject]` generator) are local foundation projects.

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
| ApplicationConfig key | PascalCase, no dots | `AppKey = "TenantTrix"` |
| Error Code | UPPER_SNAKE_CASE | `PERSON_NOT_ACTIVE`, `AUTH_CODE_NOT_VALID` |
| DB Schema | UPPER_SNAKE_CASE of the Bounded Context name (plural) — **no subdomain prefix** | `PERSONS`, `EXTERNAL_APIS`, `AXIS_IDENTITIES` |
| DB Table | `{SCHEMA}.{NAME}` UPPER_SNAKE_CASE | `PERSONS.PERSONS`, `AXIS_IDENTITIES.AXIS_IDENTITY_CELLPHONES` |
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
| Factory / AggregateApplication interfaces (same-file pair) | `internal interface` |
| Ports, BC Mediators (SDK) interfaces, SharedKernel interfaces | `public interface` |
| SharedKernel `ApplicationConfig`                           | `public static class` |
| DI extensions (public modules)                             | `public static class` |
| DI extensions (internal modules)                           | `internal static class` |
| Value Objects                                              | `readonly partial record struct` |

> **Interface scope rule**: an interface is `public` only when it crosses a project boundary (ports implemented by adapters, SDK mediators consumed by other BCs, SharedKernel contracts read by the Domain layer). Interfaces that live in the same file as their sole implementation (Factory, AggregateApplication, in-module services) stay `internal` — they are implementation details of the Application project.

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

> **AxisResult API Reference**: For the full composition methods API (`Then`, `Map`, `Tap`, `Zip`, `Recover`, `RequireNotFound`, `Combine`, `Match`, error types, etc.), see [`AxisResult.md`](AxisResult.md) at the repo root or the [NuGet package](https://www.nuget.org/packages/AxisResult).

#### Monadic composition — chaining dependent calls

When a step depends on the result of a previous step, NEVER unwrap with `if (result.IsFailure)`. Compose with the monadic operators so failure short-circuits automatically.

**`.ThenAsync()`** — chain a failable call that replaces the value:
```csharp
// ❌ WRONG — if/else flow control
var cellphoneResult = await cellphonesMediator.GetByCellphoneNumberAsync(q);
if (cellphoneResult.IsFailure)
    return AxisResult.Error<TResponse>(cellphoneResult.Errors);
var entity = await readerPort.GetByCellphoneIdAsync(cellphoneResult.Value.CellphoneId);
return new TResponse { ... };

// ✅ RIGHT — monadic composition
return await cellphonesMediator.GetByCellphoneNumberAsync(q)
    .ThenAsync(cellphone => readerPort.GetByCellphoneIdAsync(cellphone.CellphoneId))
    .MapAsync(entity => new TResponse { Id = entity.Id, Name = entity.Name });
```

**`.ZipAsync()`** — when the final mapping needs BOTH the previous value and the new one, use `.ZipAsync()` so both are passed to `.MapAsync()`:
```csharp
return await cellphonesMediator.GetByCellphoneNumberAsync(q)
    .ZipAsync(cellphone => readerPort.GetByCellphoneIdAsync(cellphone.CellphoneId))
    .MapAsync((cellphone, entity) => new TResponse
    {
        AxisIdentityId = entity.AxisIdentityId,
        CellphoneId = cellphone.CellphoneId
    });
```

Rule of thumb: `.ThenAsync()` discards the previous value, `.ZipAsync()` keeps it. Use `_` in the lambda tuple when a value is intentionally unused.

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

**Cross-field validation**: use `DependentRules<T1,T2>` for two-property validation with domain logic (failure uses the second error code). Use `NotNullOrEmpty(expr, code, Action<TProperty>)` to chain a validated value into a subsequent rule. For concrete usage, read any `*Validator.cs` under `UseCases/`.

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

### Architectural exceptions (Services, Specifications, etc.)

Handlers, Validators, Factories, and AggregateApplications cover the vast majority of application-layer concerns. When a responsibility does NOT fit any of them — typically infrastructure-adjacent orchestration that would pollute a Factory/AggregateApplication and break SRP — create a scoped helper under a dedicated folder inside the BC:

- `{Aggregate}/Services/` — stateless orchestration that the AggregateApplication or Handler depends on but that is not a domain rule (e.g., cached secret resolvers, idempotency coordinators, multi-port read compositions).
- `{Aggregate}/Specifications/` — composable query/filter predicates when read paths become too branchy for handler-only logic.
- Other folders may emerge for specific needs — if you introduce one, document its purpose in this section in the same PR.

Rules for any such helper:
- Interface + implementation in the **same `.cs` file** (`internal interface` + `internal class`) — same scope rule as Factory/AggregateApplication.
- Scoped DI lifetime, registered in the BC's `DependencyInjection.cs`.
- Primary constructor for dependencies; no `throw`; return `AxisResult` when the operation can fail.
- Before creating one, ask: can this live inside the AggregateApplication (domain-coordinating) or a port (technology-facing) instead? Prefer those. A Service is the last resort when neither fits.

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
- Each BC has its own schema, named after the BC itself in UPPER_SNAKE_CASE (plural) with **no subdomain prefix** (e.g., `PERSONS`, `EXTERNAL_APIS`, `AXIS_IDENTITIES`). Defined as `const string Schema` in the aggregate's `{Aggregate}DbInit.cs`.

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
- Handlers mirror: `Application/{BC}/UseCases/{Action}/v1/`
- Namespace follows folder. The Contracts **project** name depends on whether the BC is still part of the monolith or has been promoted to its own microservice:
  - **Default (monolith)**: a single Contracts project per subdomain — `{SubDomain}.Contracts` — holding every BC. Namespace: `{SubDomain}.Contracts.{BC}.{Feature}.{Action}.v1` (e.g., `TenantTrix.Contracts.Persons.Cellphones.AddCellphone.v1`).
  - **BC promoted to microservice**: that BC gets its own Contracts project — `{SubDomain}.{BC}.Contracts`. Namespace: `{SubDomain}.{BC}.Contracts.{Feature}.{Action}.v1` (e.g., `TenantTrix.Persons.Contracts.Cellphones.AddCellphone.v1`).
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

## Concrete Code Examples

For up-to-date scaffolds, read a sibling example first and match the exact pattern. Key locations:

| What | Where |
|------|-------|
| Command + Handler + Validator | `src/SaaS/TenantTrix/Core/TenantTrix.Application/ExternalApis/UseCases/AddExternalApi/v1/` |
| Query + Handler | `src/SaaS/TenantTrix/Core/TenantTrix.Application/ExternalApis/UseCases/GetExternalApiById/v1/` |
| Entity (Properties + Rules) | `src/SaaS/TenantTrix/Core/TenantTrix.Domain/ExternalApis/Root/` |
| AggregateApplicationFactory + AggregateApplication | `src/SaaS/TenantTrix/Core/TenantTrix.Application/ExternalApis/` |
| Repository + DbEntity + Table | `src/SaaS/TenantTrix/Adapters/Driven/Repositories/TenantTrix.Driven.Repositories.Postgres/ExternalApis/` |
| DbInit + Migrations | same Postgres folder (`*DbInit.cs` / `*Migrations.cs`) |
| SDK Mediator | `src/SaaS/TenantTrix/Adapters/Driving/TenantTrix.Sdk.Application/` |
| Cross-field validation (`DependentRules`) | any `*Validator.cs` under `DataPrivacyTrix.Application/Cellphones/UseCases/` |
| Unit tests | `TenantTrix.UnitTests/` |
| Integration tests (TestContainers) | `TenantTrix.IntegrationTests/` |

When scaffolding something new:
1. Glob for a sibling use case of the same kind (command vs query, with/without domain validation).
2. Read it end-to-end (Contract → Handler → Validator → Factory → Application → Entity → Port → Repository → SDK).
3. Mirror file structure, naming, access modifiers, and composition style.

---

## Playbook: Adding a property to an aggregate

When adding a new property (primitive or Value Object) to an existing aggregate, touch these files in order. Read sibling examples only if unsure about a specific pattern — the list itself is authoritative.

1. **SharedKernel interface** — `I{Entity}EntityProperties.cs`: add the getter.
2. **Domain entity** — `{Entity}EntityProperties.cs`: add ctor parameter, backing property, and propagate in the rehydration ctor `this(properties.X, ...)`.
3. **AggregateApplicationFactory** — `{Name}AggregateApplicationFactory.cs`: add to `NewArgs` record; pass to `new {Entity}Entity(...)` in `CreateAsync`.
4. **Command / Query** — `{UseCase}Command.cs` / `{UseCase}Query.cs`: add as nullable primitive (`string?`, `int?`) when exposed externally.
5. **Validator** — `{UseCase}Validator.cs`: add the matching validation rule (`RequiredGuid7`, `RequiredWithMaxLength`, `RequiredTryParse`, etc.) with an UPPER_SNAKE_CASE error code.
6. **Handler** — `{UseCase}Handler.cs`: forward the command property into the factory's `NewArgs`; include the property in the Response mapping if the Response exposes it.
7. **Response** — `{UseCase}Response.cs`: add `required` property if the caller needs to read it back.
8. **Repository + DbEntity + Table** — in the Postgres adapter:
   - `{Entity}DbEntity.cs`: add to record signature, update `FromReader` ordinal.
   - `{Entities}Table.cs`: add column constant; update `V1` DDL (no new migration version needed — see note below).
   - `{Entities}Repository.cs`: update `Select` constant, `INSERT` column list + `AddWithValue`.
   - `{Aggregate}DbInit.cs`: update seed `INSERT` statements (V2) if the column is `NOT NULL` and seeds exist.
9. **Tests** — update every `Mock{Entity}Properties` implementing `I{Entity}EntityProperties` (search the whole test project), plus any command/query construction in unit + integration tests. Add an `Assert.NotEmpty`/`Assert.Equal` for the new property.

**Migrations note (while local-only)**: the codebase has not shipped to any shared environment yet. Edit `V1` / seed scripts directly instead of creating incremental `V2`/`V3` migrations. When the first shared environment is introduced, update this section to require incremental versioning.

**Parallelize reads**: when starting the playbook, read all 9 touchpoints in a single batched tool call rather than sequentially.

**Finish with build + tests**:
```bash
dotnet build AxisTrix.slnx --nologo -v minimal
dotnet test AxisTrix.slnx --nologo --no-build -v minimal
```
