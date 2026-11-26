# Foundation Architecture

## Backend

The backend is a .NET 10 (LTS) Clean Architecture solution. Dependencies point inward:

```text
Domain  ←  Application  ←  Infrastructure  ←  API
                      ↖_______________________/
```

- **Domain** owns constants such as `AppRoles` and the `CurrentUser` model. It has no package or project dependencies.
- **Application** owns DTOs, interfaces, validation, and use cases (`AuthService`, `CurrentUserService`). It depends only on Domain.
- **Infrastructure** implements application interfaces using EF Core, ASP.NET Core Identity, and JWT. It owns `AppDbContext`, migrations, and seed logic.
- **API** is the composition root: controllers, middleware, DI registration, CORS, Swagger, health checks, and startup seeding. It references Application and Infrastructure for wiring only.

Business rules stay in Domain/Application. Controllers only translate HTTP requests/responses; Infrastructure never makes authorization decisions.

### Request flow example: `/api/me`

1. `CurrentUserController` reads the user ID from validated JWT claims.
2. It calls `ICurrentUserService` from Application.
3. `CurrentUserService` uses Domain/application logic and `ICurrentUserReader`.
4. `CurrentUserReader` in Infrastructure queries `AppDbContext` via EF Core.
5. The controller wraps the result in `ApiResponse<CurrentUserDto>`.

The same layering applies to login: `AuthController` → `IAuthService` → `IIdentityService`/`IJwtTokenGenerator` implementations in Infrastructure.

### Cross-cutting behavior

- `ApiResponse<T>` carries `succeeded`, `data`, `message`, and `errors` for API endpoints; `/health` remains plain.
- `[ApiController]` model validation is remapped to `ApiResponse<object>` with HTTP 400.
- `ExceptionHandlingMiddleware` maps exceptions to safe responses without exposing stack traces or sensitive data.
- JWT validation checks issuer, audience, lifetime, and signing key. `AddIdentityCore` is used instead of `AddIdentity` so Identity cookie schemes do not replace JWT as the default scheme.
- The current foundation seeds six legacy roles and may create a development
  Manager account when both seed variables are supplied. This is an
  implementation-status fact, not the approved target role model. The
  requirements document governs target registration and permissions; the
  initial Super Admin target must be provisioned by secure, idempotent seeding
  from configuration or secure secrets. This documentation update does not
  change the current implementation.
- CORS allows `http://localhost:5173` by default and is configurable via `Cors:AllowedOrigins`.

## Frontend

The React app is organized by responsibility:

- `src/app` — router and app composition.
- `src/features/auth` — login page, auth context, protected route, role route.
- `src/features/dashboard`, `src/features/manager`, `src/features/access` — pages.
- `src/components` — reusable presentational components.
- `src/layouts` — authenticated shell.
- `src/services` — Axios instance, auth/storage helpers, error extraction, API functions.
- `src/types` — shared DTO and role types.

All backend calls use one Axios instance. A request interceptor attaches the in-memory bearer token; a response interceptor clears auth state on non-login `401` responses. `AuthProvider` owns user state, and route guards redirect unauthenticated users to `/login` or forbidden roles to `/access-denied`.

Frontend guards are UX only. The API remains authoritative: every protected endpoint is independently authorized on the backend.

SANAD is an Arabic platform. New and existing approved UI must support Arabic
text and right-to-left (RTL) layout, including shared layouts, navigation,
forms, tables, validation/error/success/empty states, dates and numbers where
localized, reports, notifications, and accessibility text. Reuse the existing
application structure and shared components rather than creating a separate
Arabic frontend. Backend/API text and database fields must preserve
Arabic/Unicode content. English or additional-language support is not currently
approved and remains a localization decision requiring stakeholder approval.

## Data

SQL Server is the only application database. `InitialIdentity` creates ASP.NET Core Identity tables. Integration tests replace the SQL Server provider with EF Core InMemory via `WebApplicationFactory`; the production composition is unchanged.

Future business modules remain in this same application and database. Their
ERD-verified ownership, shared references, Identity mapping, migration
coordination, and unresolved decisions are maintained in
[`../integration/module-contracts.md`](../integration/module-contracts.md).
The approved ERD rules also require string Identity keys, GUID business keys,
same-case allocation validation, exactly-one ledger references, type-specific
notification references with null unrelated references, preserved notification
delivery history, generic append-only audit records, and selective soft
deletion with consistent `IsDeleted`/`DeletedAt` values. Audit actor nullability
for system-generated events and notification retention/archival remain
unresolved. These remain design constraints until the business modules are
implemented.

## Integration and migration workflow

The repository uses `main` as the stable release branch, `develop` as the
shared integration branch, and short-lived `feature/*` branches. Developers
implement their assigned module and create required EF Core migrations on their
own feature branch. Before opening a pull request, they merge the latest
`develop`, resolve code/migration/model-snapshot conflicts, apply migrations to
their local SQL Server database, and run relevant unit and integration tests.
Pull requests target `develop`; only tested stable releases move to `main`.

There is one SQL Server database, one shared `AppDbContext`, one migration
history, and one shared model snapshot. Developers must not blindly overwrite,
delete, or modify another developer's migrations or snapshot. The project lead
reviews entity/configuration correctness, migration operations, snapshot
consistency, sequential applicability, data integrity, security, privacy,
tests, and Clean Architecture compliance, and coordinates verification of the
combined migration sequence before it reaches the shared development database.

## Deliberate omissions

No business-module entities, migrations, endpoints, or UI are included. There is no refresh-token flow, public registration, payment integration, or microservice boundary in the foundation.
