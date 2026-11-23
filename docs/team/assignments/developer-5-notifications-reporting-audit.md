# Developer 5 — Notifications, Reporting & Audit

## 1. Purpose and responsibilities

Own notification outbox records and delivery history, reports, and generic
append-only audit records. Integrate with events and identifiers from other
modules without duplicating their entities or inventing delivery, reporting,
or retention requirements.

## 2. Entity ownership and ERD relationships

Own exactly:

- `NOTIFICATIONOUTBOX`
- `REPORT`
- `AUDITLOG`

`NOTIFICATIONOUTBOX.UserId` is required. Its type determines exactly one
business reference: `DonationImpact` → `DonationId`, `PayoutImpact` →
`PayoutId`, `CaseStatus` → `CaseId`, `SpecificNeedCovered` →
`SpecificNeedId`, and `MonthlyReminder` → no business reference. All unrelated
business reference columns are null. Notification records are preserved with
delivery status/history; retention and archival are separate decisions.

`REPORT` references the Identity generator and follows selective soft deletion.
`AUDITLOG` remains generic through controlled `EntityName` and `EntityId`; it
does not receive a foreign key for every business entity. Audit records are
append-only and may be system-generated. `UserId` nullability remains
unresolved.

## 3. Complete user-facing workflows

The approved requirements establish notifications, reporting, and audit trails.
The ERD establishes notification types/channels/statuses, report metadata, and
generic audit values, but not delivery providers, retry behavior, report
formats, audit retention, or privacy procedures. No delivery-attempt entity is
introduced without an approved decision.

Deliver complete vertical workflows for approved notification, reporting, and
audit use cases: backend persistence and rules, APIs, authorization, frontend
interfaces, API integration, state handling, and tests must work together. Do
not deliver isolated APIs or frontend mockups. Do not invent delivery,
reporting, or retention behavior.

## 4. Backend deliverables and API surface

Implement only the required Domain entities/rules, Application use cases,
services, DTOs and validation, Infrastructure configurations and persistence,
API endpoints and authorization, and focused backend tests. No business-module
endpoint paths are approved. Reports, notification operations, and audit access
must be defined only through approved use cases and authorization decisions.
Preserve existing response, validation, error, logging, and backend
authorization conventions.

## 5. Frontend deliverables

Implement the approved notification, report, and audit pages/routes and
module-specific components, forms or views, Axios integration, role-aware UI
access, and loading, empty, error, success, and validation states. Use Arabic
text and RTL layout and preserve Unicode report, notification, and audit text. Reuse the
existing layouts, navigation, auth context, shared components, and types.
Frontend checks are UX only; backend authorization is authoritative.

## 6. Shared frontend integration and dependencies

Module 5 integrates with Modules 1–4 through agreed identifiers and contracts:
case status, specific-needs, donation, and payout events may drive notification
records; reports and audit events may describe operations across modules. The
other modules retain ownership of their entities and business rules. Do not
introduce HTTP or microservice communication; the system uses one application
and shared database.

Consume other modules through agreed APIs and identifiers. Coordinate shared
frontend changes with the project lead and use actual APIs for final
integration. Do not duplicate other modules' entities or business logic.

## 7. Required backend and frontend tests

Test exact notification type/reference validation, null unrelated references,
required recipient behavior, preserved delivery status/history, report
authorization and soft deletion, generic audit identifiers, append-only
behavior, system-generated events, and sensitive-value handling. Verify local
SQL Server migrations and relevant backend unit/integration tests. Add frontend
component/integration tests for approved pages, routes, API states, role-aware
access, and notification/report/audit user interactions.

## 8. Mandatory backend implementation order

Follow **Analyze → Domain → Application → Infrastructure → API → Testing →
Review** and the shared [module contracts](../../integration/module-contracts.md).
Only implement layers required by approved requirements.

## 9. Git and EF Core migration workflow

Work on `feature/*`, create and test required migrations there, apply them in
order to local SQL Server, merge the latest `develop` before the PR, and
reconcile migration/model-snapshot conflicts with the project lead. Target the
PR at `develop`; never push directly to `develop` or `main`.

## 10. Definition of Done

- Owned entities and configurations match the approved ERD.
- Required backend and frontend workflows are integrated end to end.
- Frontend uses shared conventions and actual agreed APIs.
- Notification type/reference rules and nullability are enforced.
- Notification history is preserved without an invented delivery-attempt table.
- Audit remains generic, append-only, privacy-conscious, and system-event aware.
- Cross-module identifiers and event contracts are coordinated.
- Local migrations and tests pass after integrating the latest `develop`.
- PR documents retention, privacy, API, database, and testing impacts.

## 11. Unresolved decisions

Notification delivery provider/retry details, retention and archival periods,
report formats, audit actor nullability, audit retention/privacy procedures,
authorization, and API endpoint shapes require approval.
