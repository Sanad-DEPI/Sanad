# SANAD — Project Instructions

## Context
SANAD (SACMP — Smart Aid & Charity Management Platform) manages family registration and case management, identity validation, document management and verification, needs assessment and scoring, donations and payments, fund allocation, payout authorization and execution, notifications, reporting, and audit trails. It handles sensitive beneficiary data and financial operations.

## Agreed technology
- Backend: .NET 10 LTS ASP.NET Core Web API, Clean Architecture
- Frontend: React + TypeScript + Vite; Axios for REST calls
- Data: SQL Server with EF Core; ASP.NET Core Identity with JWT
- Monorepo; 5 developers; approximately 10 days
- Platform language: Arabic, with RTL user interfaces required

## Layout
- `backend/src/SANAD.API`, `SANAD.Application`, `SANAD.Domain`, `SANAD.Infrastructure`; `backend/tests/SANAD.UnitTests`, `SANAD.IntegrationTests`
- `frontend/` — Vite React + TypeScript application
- `backend/src/SANAD.API/Controllers/CurrentUserController.cs` (`/api/me`) is the reference backend feature; follow its layer flow for new backend work.

## General principles
- Inspect existing code and conventions before modifying anything. Follow them instead of introducing a personally preferred style.
- Change only files related to the request. Explain modified files and important decisions.
- Clarify missing requirements. Do not invent business rules or architectural decisions.
- Before implementing or reviewing any registration, account, case, document,
  scoring, donation, allocation, payout, report, or notification behavior,
  consult `docs/requirements/requirements.md` Sections 4–5 for the authoritative
  account types, role responsibilities, privacy boundaries, and workflows.
  Check each data field and action against the actor's approved scope; do not
  infer or expand permissions. Keep any undecided access explicitly marked
  `UNRESOLVED — REQUIRES DECISION` and deny it until approved.
- Prefer simple, explicit solutions. Avoid unnecessary abstractions, packages, and files.
- Verify before claiming success: build and test what you changed, and report exactly what was and was not run.
- Respect module ownership. Before changing another module or a shared contract, identify the dependency and coordinate with its owner. The five business modules are not implemented yet; do not add them without an explicit request.
- Read `docs/integration/module-contracts.md` before implementing or reviewing a business-module feature. It is the central ownership, shared-reference, migration, and collaboration contract.
- Create required migrations on your feature branch, apply them to your local
  SQL Server, and coordinate snapshot/migration conflicts with the project
  lead. Never overwrite or delete another developer's migrations.

## Backend work
- Follow the mandatory implementation order and dependency rules in `backend/AGENTS.md` (also in `.agents/skills/backend-feature/SKILL.md`): Analyze → Domain → Application → Infrastructure → API → Testing → Review. Only touch the layers a feature actually needs.

## Security and privacy
- Treat beneficiary data as sensitive: minimize exposure in logs, responses, and commits.
- Enforce authorization and validation on the backend; never trust frontend checks.
- Preserve separation of duties and financial consistency. Do not bypass approval or audit flows.
- Apply least privilege to every role, API response, document, report, export,
  notification, and log. Role names, module ownership, and frontend route
  guards do not grant data access.
- Never commit secrets, credentials, tokens, or local configuration.

## Collaboration
- Work on short-lived feature branches and open a pull request for review. Do not commit directly to `main`.
- Use `develop` as the shared integration branch. Work on `feature/*` branches,
  merge the latest `develop` before opening a PR, and target PRs at `develop`.
- Developers may create required EF Core migrations on their feature branches,
  but must reconcile migration/model-snapshot conflicts and apply migrations to
  their own local SQL Server. The project lead coordinates integrated migration
  history; do not overwrite or delete another developer's migrations.
- Follow `.agents/rules/git-workflow.md` and `.agents/rules/testing.md`.
- Keep changes small enough to review within the short timeline.
