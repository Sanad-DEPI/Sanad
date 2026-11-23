# Backend Instructions (.NET 10 LTS)

Applies to `backend/`. Also follow the root `AGENTS.md`.

## Solution and dependency direction
`backend/SANAD.sln`:

- `src/SANAD.Domain` — entities, value objects, enums, domain rules. No project dependencies.
- `src/SANAD.Application` — DTOs, interfaces, application services, validation. Depends on Domain only.
- `src/SANAD.Infrastructure` — EF Core (SQL Server), Identity, JWT, external integrations. Depends on Application and Domain.
- `src/SANAD.API` — controllers, middleware, configuration, DI, startup. Depends on Application and Infrastructure for composition only.
- `tests/SANAD.UnitTests`, `tests/SANAD.IntegrationTests` — xUnit tests.

Strict rules: Domain depends on nothing; Application never depends on API or Infrastructure; business rules belong in Domain/Application, never in controllers or Infrastructure. Infrastructure implements abstractions owned by inner layers.

## Conventions
- .NET 10 / C#: nullable reference types enabled, file-scoped namespaces, implicit usings, async/await with `CancellationToken` where supported.
- Folders and names: feature folders under Application (e.g. `Features/CurrentUser`), request/response DTO suffixes, `I`-prefixed interfaces, singular entity names.
- Validation: DataAnnotations on request DTOs with `[ApiController]` automatic model validation; application services re-check required invariants for non-HTTP callers.
- Errors: throw from inner layers; `ExceptionHandlingMiddleware` maps exceptions to a consistent `ApiResponse<T>` with correct HTTP status codes. Never leak stack traces or sensitive data.
- Responses: use `Application/Common/ApiResponse.cs` (`succeeded`, `data`, `message`, `errors`); the `/health` endpoint stays plain.
- DI: register services in API startup (or extension methods beside it), constructor injection only, no service locator.
- EF Core: `AppDbContext` lives in Infrastructure; use `IEntityTypeConfiguration<T>` when mapping is needed; add migrations with `dotnet ef migrations add <Name> --project src/SANAD.Infrastructure --startup-project src/SANAD.API` on the feature branch when required; apply them to the developer's local SQL Server, reconcile snapshot conflicts with the project lead, and commit migrations without secrets.
- Auth: ASP.NET Core Identity + JWT. Role names come only from `SANAD.Domain/Constants/AppRoles.cs`. Protect endpoints with `[Authorize]` / `[Authorize(Roles = ...)]`. Backend authorization is authoritative.
- Logging: `ILogger<T>`. Never log beneficiary data, passwords, tokens, or connection strings.
- Arabic/Unicode: API DTOs and EF Core text handling must preserve Arabic
  content. Do not assume Latin-only names or fields. User-visible API messages
  must support the approved Arabic UI localization decision; English or
  additional-language support remains unresolved.
- Testing: xUnit. Unit tests for domain/application logic; integration tests with `WebApplicationFactory` for endpoints, authentication, and role authorization. EF Core InMemory is allowed in tests only.

## Mandatory implementation order (every backend feature)
1. **Analyze** — requirements, business rules, module ownership, dependencies, existing implementation.
2. **Domain** — entities, value objects, enums, domain rules, only if the feature needs them.
3. **Application** — use cases, DTOs, interfaces, validation, application logic.
4. **Infrastructure** — EF Core mappings, repository/service implementations, external integrations, only if needed.
5. **API** — endpoints, controllers, request handling, registrations.
6. **Testing** — add and execute appropriate unit and integration tests.
7. **Review** — architecture compliance, consistency, security, build results.

Do not create classes in every layer automatically. Only implement what the feature requires.

## Module ownership
Before backend work, read `../docs/integration/module-contracts.md`. Do not modify another module's implementation or shared contracts without checking ownership and coordinating with the responsible developer. If a feature requires changes elsewhere, identify the dependency and explain the required contract changes before proceeding.

The application uses one shared `AppDbContext`, one migration history, and one
shared model snapshot. Developers may create migrations independently on
feature branches, but must never blindly overwrite, delete, or modify another
developer's migrations or snapshot. Merge the latest `develop` before opening a
PR, resolve conflicts properly, and target the PR at `develop`. The project
lead reviews and coordinates integrated migration correctness; this does not
make the lead responsible for implementing another developer's Infrastructure
code.

When implementing the planned modules, preserve the ERD contracts in
`docs/integration/module-contracts.md`: Identity and all Identity foreign keys
use string keys, business entities use GUID keys, allocations enforce the
same-case need rule, ledger entries enforce exactly one source reference, and
notification references are validated from their type. Apply selective soft
deletion only where documented; never physically delete financial, audit, status
history, or approval records through normal operations.

## Mandatory completion checklist
- Requirements and business rules satisfied.
- Changes in correct layers; dependency direction respected.
- No unnecessary abstractions or duplicated logic.
- Validation and error handling covered.
- Authorization applied where required.
- Database changes and migrations correct.
- Relevant tests pass and the solution builds.

Report any uncompleted check honestly.

## Reference feature
`CurrentUser` (`/api/me`) is the agreed style example: Domain model → Application DTO/interface/service → Infrastructure reader → API controller → integration test. Copy this flow; do not invent a new pattern.

## Scope
Only the authentication foundation and the reference feature exist. The five business modules (case management, documents/assessment, donations/payments, allocation/payouts, notifications/reporting/audit) and their tables are not implemented yet.
