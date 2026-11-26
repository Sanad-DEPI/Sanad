# SANAD (SACMP)

Smart Aid & Charity Management Platform — a charity management system planned for family registration and case management, identity validation, document verification, needs assessment and scoring, donations and payments, fund allocation, payout authorization and execution, notifications, reporting, and audit trails.

**Status:** the base application is implemented and verified locally. The five business modules are intentionally not implemented yet.

**Language and direction:** SANAD is an Arabic platform. All approved user-facing
screens, labels, validation messages, empty/error/success states, navigation,
and accessibility text must support Arabic and use right-to-left (RTL) layout.
The backend must preserve UTF-8/Unicode text. Whether English or other languages
are also supported remains an unresolved localization decision; do not add
other-language requirements or translations without approval.

This README is the entry point for all developers and AI agents. Read
[`docs/integration/module-contracts.md`](docs/integration/module-contracts.md)
before implementing or reviewing a business feature. For any account,
registration, authorization, beneficiary-data, or financial workflow, also
read Sections 4–5 of
[`docs/requirements/requirements.md`](docs/requirements/requirements.md), the
authoritative source for roles and permission boundaries. Do not infer or
expand permissions; unresolved access remains denied until approved.

## What is implemented

### Backend
- .NET 10 (LTS) ASP.NET Core Web API in a Clean Architecture solution.
- ASP.NET Core Identity with the six legacy foundation roles, SQL Server/EF
  Core, and JWT bearer authentication. These existing roles are implementation
  status, not the approved target account/role model.
- `POST /api/auth/login`, protected `GET /api/me`, `GET /api/test/authenticated`, and a foundation-only Manager test endpoint.
- Consistent `ApiResponse<T>` envelope, global model-validation mapping, exception middleware, CORS, Swagger with JWT support, health check, and structured logging.
- EF Core migration `InitialIdentity` for Identity tables and role seed data.
- xUnit unit and integration tests for health, login, validation, authorization, and the reference `/api/me` feature.

### Frontend
- React + TypeScript + Vite app with Axios, React Router, and React Testing Library/Vitest.
- Login form with client-side validation and readable API errors.
- Auth context with an in-memory access token, protected layout, Manager role guard, access-denied and not-found handling, logout, dashboard, and Manager page.
- Responsive, accessible, professional-looking starter UI with loading, error, empty, and success states.

### Agent/tooling setup
- Shared `AGENTS.md`, `.agents/rules/`, and `.agents/skills/` are the source of truth.
- `.github/copilot-instructions.md` and path-specific instructions point at the real architecture.
- `opencode.jsonc` remains the OpenCode adapter for the shared rules/skills.

## Repository structure

- `backend/SANAD.sln` — six projects:
  - `src/SANAD.Domain` — entities, constants, domain rules; no dependencies.
  - `src/SANAD.Application` — DTOs, interfaces, use cases; depends on Domain only.
  - `src/SANAD.Infrastructure` — EF Core, Identity, JWT; depends on Application and Domain.
  - `src/SANAD.API` — controllers, middleware, DI, startup; composition only.
  - `tests/SANAD.UnitTests` — domain/application tests.
  - `tests/SANAD.IntegrationTests` — `WebApplicationFactory` endpoint tests.
- `frontend/` — Vite app; feature code under `src/features`, shared UI under `src/components`, API calls under `src/services`, router/setup under `src/app`.
- `docs/` — project overview, requirements, architecture, and decision records.
- `.agents/rules/` — git workflow and testing expectations.
- `.agents/skills/` — backend/frontend feature, code-review, and debugging workflows.
- `.github/` — Copilot adapters and issue templates.
- `opencode.jsonc` — OpenCode adapter.

## Planned development modules

The five modules below are logical ownership boundaries in one backend
application and one shared database. They are planning allocations; their
entities and tables are not implemented in the current foundation.

| Module | Ownership |
| --- | --- |
| Identity & Case Management | `FAMILY`, `FAMILYMEMBER`, `CASE`, `CASESURVEY`, `CASESTATUSHISTORY` |
| Document Verification & Assessment | `DOCUMENT`, `SCORINGRULESET`, `SCORERESULT`, `SPECIFICNEED` |
| Donations & Payments | `DONOR`, `DONATIONCATEGORY`, `DONATION`, `PAYMENTTRANSACTION` |
| Fund Allocation & Payouts | `DONATIONALLOCATION`, `LEDGERENTRY`, `PAYOUT`, `PAYOUTAPPROVAL` |
| Notifications, Reporting & Audit | `NOTIFICATIONOUTBOX`, `REPORT`, `AUDITLOG` |

The ownership is verified against
[`docs/architecture/sanad_erd.mermaid`](docs/architecture/sanad_erd.mermaid).
See the [module contracts](docs/integration/module-contracts.md) for the
complete ERD inventory, shared references, relationship cardinalities,
Identity decisions, integrity constraints, and remaining design questions.
The approved ERD links each Beneficiary Identity user to exactly one family
and represents guest donations without a Donor account or category; these
documentation decisions are not yet implemented in the application or schema.

## Module dependencies and shared data

Identity owns authentication users, roles, and user-role relationships through
`AspNetUsers`, `AspNetRoles`, and `AspNetUserRoles`. Do not create duplicate
authentication tables. Verified cross-module references include `CASE` from Modules 1, 2, and 4,
`SPECIFICNEED` from Module 2 to Module 4, and `DONATION` from Module 3 to
Module 4. Module 4 owns `PAYOUT` and `LEDGERENTRY`; `NOTIFICATIONOUTBOX`
preserves delivery status/history and supports exactly one corresponding
donation, payout, case, or specific-need reference selected by notification
type; unrelated references are null and `MonthlyReminder` has none. Identity
`USER` is referenced throughout the modules.
Identity and all Identity foreign keys use string keys; business entities use
GUID keys. The shared database and agreed contracts do not imply five services
or five databases. Allocation same-case validation, ledger reference
constraints, notification type validation, audit immutability, and selective
soft deletion are defined in the
[module contracts](docs/integration/module-contracts.md).

The owning developer controls an entity's implementation and configuration.
Consumers must use the agreed entity/contract and coordinate changes with its
owner instead of duplicating or independently reshaping it.

## Standard development workflow

For backend work, follow **Analyze → Domain → Application → Infrastructure →
API → Testing → Review** and the dependency rules in
[`backend/AGENTS.md`](backend/AGENTS.md). Only create the layers a feature
needs. Frontend work follows the existing React structure, shared Axios client,
auth context, and accessible loading/error/success states.

Database work uses one SQL Server database, one shared `AppDbContext`, one
coordinated EF Core migration history, and one shared model snapshot. Developers
own feature-branch migrations required by their module and apply them in order
to their own local SQL Server. They must reconcile migration and snapshot
conflicts with the project lead rather than overwriting or deleting another
developer's work. The project lead reviews migration operations, snapshot
consistency, sequential applicability, integrity, security, privacy, tests,
and Clean Architecture compliance before integrated database updates. See the
[database workflow](docs/integration/module-contracts.md#database-and-migrations).

## Branch and pull request workflow

Use `main` as the stable release branch, `develop` as the shared integration
branch, and short-lived `feature/*` branches for module work. Do not push
directly to `develop` or `main`. Before opening a PR, merge the latest
`develop` into the feature branch, resolve code/migration/snapshot conflicts,
apply migrations to the local SQL Server, and run relevant tests. Open the PR
against `develop`; promote only tested stable releases to `main`. PRs must
confirm ownership, architecture, tests, database coordination, API contract
changes, and no unrelated modifications.

## Agreed technology

- Backend: .NET 10 (LTS), ASP.NET Core Web API, Clean Architecture.
- Frontend: React + TypeScript + Vite, Axios, React Router.
- Data: SQL Server with EF Core; ASP.NET Core Identity + JWT.
- Repository: monorepo with two independently runnable applications.

## Prerequisites

- .NET 10 SDK. The machine-local verification used .NET SDK `10.0.404`; the older `9.0.316` system SDK cannot build `net10.0`.
- SQL Server reachable from the API. The `appsettings.json` default uses local Windows integrated authentication with `TrustServerCertificate=True`.
- Node.js 22+ and npm (verification used Node `v22.20.0`, npm `10.9.3`).

## Configuration

### Backend

`backend/.env.example` is a reference checklist, **not** something ASP.NET Core loads automatically. Export the values in your shell, use `dotnet user-secrets`, or use an ignored `appsettings.Local.json`.

The API intentionally has no JWT secret in source control and fails fast if `Jwt__Secret` is missing. Example PowerShell session setup:

```powershell
$env:Jwt__Secret = 'replace-with-a-random-secret-at-least-32-characters-long'
$env:ConnectionStrings__Default = 'Server=localhost;Database=SANAD;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true'

# Legacy foundation behavior: optional, Development-only Manager bootstrap.
$env:Seed__ManagerEmail = 'manager@example.test'
$env:Seed__ManagerPassword = 'REPLACE_WITH_LOCAL_DEV_PASSWORD_123'
```

Replace the placeholder secret and password with local values; the shown strings are examples only.

The current foundation seeds its six legacy roles; without the optional
development seed settings, no user is created. The foundation has no public
registration endpoint. This current behavior is not the approved target: public
registration is limited to Donor or Beneficiary, and the initial Super Admin
must be provisioned through secure, idempotent seeding using configuration or
secure secrets. Do not commit credentials or use plaintext password storage.

Apply the Identity migration before first run:

```bash
dotnet ef database update --project backend/src/SANAD.Infrastructure --startup-project backend/src/SANAD.API
```

### Frontend

Copy the environment template and adjust it only if the API runs elsewhere:

```powershell
Copy-Item frontend/.env.example frontend/.env
```

`VITE_API_URL` defaults to `http://localhost:5045/api` when the file is absent. Vite embeds `VITE_*` values into browser bundles, so never put secrets there.

## Run locally

Backend (Swagger at `http://localhost:5045/swagger`, health at `http://localhost:5045/health`):

```bash
dotnet run --project backend/src/SANAD.API
```

Frontend (Vite dev server at `http://localhost:5173`):

```bash
cd frontend
npm install
npm run dev
```

For the current foundation only, sign in with the development-seeded Manager
account to see the dashboard and its Manager test page. This is legacy
implementation behavior; it is not the approved target role model. Other
current foundation roles are redirected to `/access-denied` for that route.

## API surface

| Method | Path | Access | Purpose |
| --- | --- | --- | --- |
| `GET` | `/health` | Anonymous | Liveness check (plain response, not `ApiResponse<T>`) |
| `POST` | `/api/auth/login` | Anonymous | Returns a JWT and the user's roles |
| `GET` | `/api/me` | Authenticated | Reference feature: current user profile |
| `GET` | `/api/test/authenticated` | Authenticated | Confirms JWT authentication |
| `GET` | `/api/test/manager` | Manager | Confirms role authorization |

## Token storage decision

The access token is stored only in React memory, not `localStorage` or `sessionStorage`. A page reload returns the user to `/login`; there is no refresh-token flow yet. Frontend route guards improve UX only — every protected API call is authorized by the backend. A `401` response clears auth state and redirects the user to login.

## Verification commands

```bash
dotnet build backend/SANAD.sln
dotnet test backend/SANAD.sln
npm --prefix frontend run build
npm --prefix frontend test
npm --prefix frontend run lint
```

Exact results of the latest run are reported in the final handoff message. Integration tests use EF Core InMemory and `WebApplicationFactory`; the real application uses SQL Server.

## Documentation

- `docs/project-overview.md` — scope, constraints, and current status.
- `docs/architecture/foundation.md` — implemented architecture and request flows.
- `docs/integration/module-contracts.md` — module ownership, shared references,
  migration coordination, workflows, and unresolved decisions.
- `docs/team/README.md` — official Developer 1–5 assignments and coordination
  guide.
- `docs/decisions/ADR-001-dotnet-10-lts.md` — framework/package decision.
- `docs/requirements/requirements.md` — detailed requirements and authoritative
  account registration, role, privacy, and workflow decisions.
- `docs/decisions/ADR-template.md` — template for future decisions.

## AI agent setup

- `AGENTS.md` at the root and in `backend/` / `frontend/` is the shared source of truth.
- Copilot: `.github/copilot-instructions.md` plus `.github/instructions/*.instructions.md`.
- OpenCode: `AGENTS.md` discovery plus `opencode.jsonc`; skills in `.agents/skills/`.
- Antigravity: root `AGENTS.md`, `.agents/rules/`, and `.agents/skills/`.

## Scope boundary

Do not add the five business modules, payment gateways, document workflows, notifications, fund allocation, payouts, or microservices without an explicit request and design decision. Authorization, validation, privacy, and auditability remain backend
responsibilities for any future module. Before implementation, compare all
role-specific actions and data fields with the approved requirements; do not
expand permissions or treat current foundation behavior as the target.
