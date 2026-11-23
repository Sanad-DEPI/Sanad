# SANAD Team Distribution

This is the official assignment index for the five SANAD full-stack developers.
Developer names are intentionally placeholders. Ownership is logical ownership
inside one backend application and one shared SQL Server database; it is not a
service or database split.

## Assignments

| Developer | Module | Owned entities | Assignment |
| --- | --- | --- | --- |
| Developer 1 | Identity & Case Management | `FAMILY`, `FAMILYMEMBER`, `CASE`, `CASESURVEY`, `CASESTATUSHISTORY` | [Developer 1 assignment](assignments/developer-1-identity-case.md) |
| Developer 2 | Document Verification & Assessment | `DOCUMENT`, `SCORINGRULESET`, `SCORERESULT`, `SPECIFICNEED` | [Developer 2 assignment](assignments/developer-2-documents-assessment.md) |
| Developer 3 | Donations & Payments | `DONOR`, `DONATIONCATEGORY`, `DONATION`, `PAYMENTTRANSACTION` | [Developer 3 assignment](assignments/developer-3-donations-payments.md) |
| Developer 4 | Fund Allocation & Payouts | `DONATIONALLOCATION`, `LEDGERENTRY`, `PAYOUT`, `PAYOUTAPPROVAL` | [Developer 4 assignment](assignments/developer-4-allocation-payouts.md) |
| Developer 5 | Notifications, Reporting & Audit | `NOTIFICATIONOUTBOX`, `REPORT`, `AUDITLOG` | [Developer 5 assignment](assignments/developer-5-notifications-reporting-audit.md) |

These assignments account for all 20 business entities in the approved ERD.
`USER`, `ROLE`, and `USERROLE` are existing ASP.NET Core Identity
infrastructure. They are not assigned as duplicate business entities to any
developer. Identity uses string IDs and business entities use GUID IDs; every
business foreign key referencing Identity uses a string.

## Responsibilities by module

Each developer owns complete vertical delivery for the assigned module:
backend entities and business logic, application use cases and DTOs,
Infrastructure and EF Core configuration, API endpoints and authorization,
frontend pages and routes, module components and forms, API integration, role-
based UI access, loading/error/empty/success states, Arabic RTL presentation,
and backend/frontend tests. A module is not complete when only its API or a
frontend mockup works;
the user-facing workflow must work end to end through the agreed contracts.

- **Developer 1:** family registration, family members, case creation and
  lifecycle, surveys, immutable case status history, and their user-facing
  workflows.
- **Developer 2:** case documents, scoring rule versions and results, priority
  assessment, specific needs, and their user-facing workflows.
- **Developer 3:** donor business records, donation categories, donations,
  payment transactions, and their user-facing workflows.
- **Developer 4:** donation allocations, ledger entries, payouts,
  multi-step payout approvals, and their user-facing workflows.
- **Developer 5:** notification outbox delivery history, reports, generic
  append-only audit records, and their user-facing workflows where approved.

## Shared frontend rules

All modules use the existing React + TypeScript + Vite application. Developers
must reuse the existing layouts, navigation, shared components, Axios client,
auth context, role-aware routing, common types, and validation/error
conventions. Module-specific code belongs under the relevant feature area where
the current structure permits it. Do not create a second authentication
implementation, duplicate shared components, introduce a new state library, or
change frontend libraries without an approved decision.

Frontend route checks improve UX only; backend authorization remains
authoritative. Every module must handle loading, empty, error, success, and
form-validation states and must include accessible, responsive UI. Developers
consume other modules through agreed API contracts and must not duplicate their
entities or backend logic. Temporary mocks are permitted only during isolated
development; final integration must use the agreed APIs.

The project lead coordinates shared layouts, navigation, authentication
integration, shared components, and global frontend conventions. Developers may
propose and implement shared changes, but must coordinate them before changing
shared frontend files. Cross-module workflows must be coordinated and tested.

## Cross-module dependencies

| Consumer | Depends on | Coordination |
| --- | --- | --- |
| Module 2 | Module 1 `CASE` | Documents, scores, and needs identify the owned case. |
| Module 4 | Module 1 `CASE` | Allocations and payouts target a case. |
| Module 4 | Module 2 `SPECIFICNEED` | An allocation may optionally target a need; if present, the need must belong to the same case. |
| Module 4 | Module 3 `DONATION` | An allocation records the source donation. |
| Module 5 | Other modules | Notification types reference their corresponding donation, payout, case, or specific need; reports and audit events consume agreed contracts and identifiers. |

Ownership remains with the assigned developer. Consumers reference the agreed
entity and contract; they do not create duplicate entities or independently
reshape another module's model. This is one application with one shared
database, not a microservice or HTTP integration boundary.

Shared integrity rules include:

- `DONATIONALLOCATION.CaseId` is required; nullable `NeedId` must point to a
  need belonging to the same case.
- Exactly one of `LEDGERENTRY.DonationId`, `AllocationId`, and `PayoutId` is
  populated, with the required `EntryType` mapping.
- `NOTIFICATIONOUTBOX` has exactly the reference required by its type; all
  unrelated business references are null. `MonthlyReminder` has none.
- Audit records remain generic, append-only, and use controlled `EntityName`
  values. Audit actor nullability remains unresolved.
- Soft deletion is selective and follows the `IsDeleted`/`DeletedAt`
  consistency rule. Financial, audit, status-history, and approval records are
  not physically deleted through normal operations.

## Project lead responsibilities

The project lead coordinates:

- shared architecture and conventions;
- cross-module contracts and integration;
- PR reviews and approvals;
- migration and model snapshot reconciliation;
- integration testing;
- final system verification.
- shared frontend infrastructure and cross-module UI integration.

Developers own their module implementations, configurations, APIs, tests, and
feature-branch migrations. The project lead coordinates the shared migration
history but does not automatically implement another developer's module or
Infrastructure code.

## Shared documentation

- [Project README](../../README.md)
- [Approved ERD](../architecture/sanad_erd.mermaid)
- [Module contracts](../integration/module-contracts.md)
- [Foundation architecture](../architecture/foundation.md)
- [Requirements](../requirements/requirements.md)
- [Testing rules](../../.agents/rules/testing.md)
- [Git and migration workflow](../../.agents/rules/git-workflow.md)
- [Backend instructions](../../backend/AGENTS.md)
- [Frontend instructions](../../frontend/AGENTS.md)
- [Backend feature skill](../../.agents/skills/backend-feature/SKILL.md)

## Shared workflow

Every developer follows the existing Clean Architecture order:

**Analyze → Domain → Application → Infrastructure → API → Testing → Review**

Use the existing validation, error handling, `ApiResponse<T>`, dependency
injection, authorization, logging, and testing conventions. Work on
`feature/*`, merge the latest `develop` before opening a PR, resolve migration
and snapshot conflicts properly, and target the PR at `develop`. Apply
migrations in order to the developer's local SQL Server and verify the feature
after integrating the latest `develop`. Run both backend and frontend
verification for the complete vertical feature.

The assignment files below explain module-specific scope without duplicating
the shared contracts.
