---
name: backend-feature
description: Implement a backend feature in the SANAD .NET 10 (LTS) Clean Architecture solution using the mandatory layer order, dependency rules, and verification checklist. Use when adding or changing backend endpoints, business rules, validation, or data access.
---

# Backend Feature

## Purpose
Implement a requested backend change in the established Clean Architecture style, with consistent layering, authorization, validation, tests, and honest verification.

## Use when
- Adding or modifying a backend capability, rule, endpoint, or persistence behavior.
- The change touches server-side validation, authorization, or financial/beneficiary data.

## Inspect first
Read root `AGENTS.md`, `backend/AGENTS.md`, `.agents/rules/testing.md`, `docs/integration/module-contracts.md`, and (for registration, authorization, case, document, scoring, donation, allocation, payout, reporting, or notification work) Sections 4–5 of `docs/requirements/requirements.md`. Open `backend/src/SANAD.API/Controllers/CurrentUserController.cs` and follow its files across Domain → Application → Infrastructure → API. Check the owning module and inspect existing entities before touching files or shared contracts. Do not assume a pattern exists just because it is common.

## Mandatory implementation order
1. **Analyze** — requirements, business rules, module ownership, dependencies, and existing implementation. Ask about gaps instead of inventing them.
2. **Domain** — entities, value objects, enums, domain rules, only if the feature needs them.
3. **Application** — use cases, DTOs, interfaces, validation, application logic.
4. **Infrastructure** — EF Core mappings/configurations, implementations of inner-layer interfaces, external integrations, only if needed.
5. **API** — endpoints, controllers, request handling, and required registrations.
6. **Testing** — add unit and/or integration tests, then execute them.
7. **Review** — check architecture compliance, consistency, security, and build results.

Only create classes in the layers the feature actually requires.

## Hard rules
- Domain depends on nothing. Application depends only on Domain. Infrastructure depends on Application and Domain. API may depend on Application and Infrastructure for composition.
- No business rules in controllers or Infrastructure. No Domain/Application reference to API or Infrastructure.
- Use existing conventions for naming, DTOs, validation (DataAnnotations), `ApiResponse<T>` errors, DI, EF Core mappings/migrations, auth (`AppRoles` constants for current foundation roles and approved requirements for target roles), and logging.
- Do not modify another module or shared contract without checking ownership and explaining the contract change first.
- Before implementing authorization, map each action and returned data field to
  the approved role permissions in `docs/requirements/requirements.md`. Apply
  least privilege and deny unresolved access until approved; never infer or
  expand permissions from an existing role name or UI.
- Create required migrations on the feature branch and apply them to the
  developer's local SQL Server. Never blindly overwrite, delete, or modify
  another developer's migrations or model snapshot. Merge the latest
  `develop`, reconcile conflicts with the project lead, and target the PR at
  `develop`.
- Implement the ERD contracts from `docs/integration/module-contracts.md`: validate allocation case/need ownership, enforce the ledger CHECK and `EntryType` mapping, validate notification references by type, preserve generic append-only audit logs, and apply only documented soft deletion.
- Do not add packages, migrations, or unrelated refactors unless the feature requires them.

## Validate before reporting
- `dotnet build backend/SANAD.sln`
- Relevant test projects; report exact commands and results.
- Review the diff for unintended files and confirm every checklist item in `backend/AGENTS.md`. Say plainly if a check was not completed.

## Final response format
- **Files changed:** list paths by layer.
- **Implementation:** brief summary of behavior and any decision made.
- **Validation:** commands run, results, and what was not verified.
- **Unresolved concerns:** questions, ownership issues, or known risks.
