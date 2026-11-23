# Developer 4 — Fund Allocation & Payouts

## 1. Purpose and responsibilities

Own allocation of donations to cases/needs, financial ledger references, payout
requests and execution, and payout approval steps. Preserve separation of
duties and financial integrity without inventing unspecified accounting rules.

## 2. Entity ownership and ERD relationships

Own exactly:

- `DONATIONALLOCATION`
- `LEDGERENTRY`
- `PAYOUT`
- `PAYOUTAPPROVAL`

`DONATIONALLOCATION` requires `CaseId`, optionally references `SPECIFICNEED`,
and references `DONATION`. If `NeedId` is populated, the need must belong to
the same case. `LEDGERENTRY` has nullable `DonationId`, `AllocationId`, and
`PayoutId`; exactly one is populated, with the approved `EntryType` mapping.
`PAYOUT` references `CASE`; `PAYOUTAPPROVAL` references `PAYOUT` and an
Identity actor. Modules 1–3 own the referenced case, need, and donation.

## 3. Complete user-facing workflows

The approved requirements establish fund allocation, payout authorization,
review, and execution. The ERD establishes statuses, methods, approval steps,
and ledger references, but not amount signs, accounting behavior, provider,
transaction behavior, authorization matrix, or state-transition rules. Do not
invent them.

Deliver complete vertical allocation and payout workflows: backend rules, APIs,
authorization, persistence, frontend interfaces, approval interactions, error
handling, and tests must work together. Do not deliver isolated APIs or
frontend mockups. Document unclear financial or external-provider behavior and
request approval.

## 4. Backend deliverables and API surface

Implement only the required Domain entities/rules, Application use cases,
services, DTOs and validation, Infrastructure configurations and persistence,
API endpoints and authorization, and focused backend tests. No business-module
endpoint paths are approved. Define allocation, ledger, and payout endpoints
only after use cases and authorization decisions are approved. Use existing DTO,
validation, `ApiResponse<T>`, error handling, DI, and backend authorization
conventions.

## 5. Frontend deliverables

Implement the approved allocation, payout, and approval pages and routes,
module-specific forms and review components, Axios API integration, role-aware
UI access, and loading, empty, error, success, validation, and approval-state
feedback. Use Arabic text and RTL layout and preserve Unicode comments and
beneficiary-facing data. Reuse existing layouts, navigation, auth context, shared components,
and types. Frontend checks are UX only; backend authorization and separation of
duties are authoritative.

## 6. Shared frontend integration and dependencies

Module 4 depends on Module 1 `CASE`, Module 2 `SPECIFICNEED`, and Module 3
`DONATION`. Coordinate identifier and lifecycle contracts with all three owners.
Developer 5 may consume payout events for notifications and generic audit
events. Do not duplicate their entities or change their structure independently.

Consume other modules through agreed APIs and identifiers. Coordinate shared
frontend changes with the project lead and use actual APIs for final
integration. Do not duplicate case, need, donation, or Identity logic.

## 7. Required backend and frontend tests

Test same-case allocation validation, nullable need behavior, ledger CHECK and
`EntryType` mapping, payout authorization/separation of duties, approval
history immutability, financial record non-deletion, concurrency and partial
failure behavior where requirements define it, and cross-module references.
Add frontend component/integration tests for allocation, payout, approval,
validation, protected routes, and API states. Verify migrations and schema
constraints on local SQL Server and after integrating `develop`.

## 8. Mandatory backend implementation order

Follow **Analyze → Domain → Application → Infrastructure → API → Testing →
Review** and the [backend feature skill](../../../.agents/skills/backend-feature/SKILL.md).
Financial rules belong in Domain/Application, not controllers or Infrastructure.

## 9. Git and EF Core migration workflow

Use `feature/*`, create and test required migrations on the branch, apply them
in order to local SQL Server, merge the latest `develop` before opening the PR,
and reconcile migration/model-snapshot conflicts with the project lead. Target
the PR at `develop`; never push directly to `develop` or `main`.

## 10. Definition of Done

- Ownership and all three cross-module contracts are coordinated.
- Required backend and frontend workflows are integrated end to end.
- Frontend uses shared conventions and actual agreed APIs.
- Same-case, exactly-one-ledger-reference, and `EntryType` rules are enforced
  at the agreed database/application boundaries.
- Separation of duties, authorization, immutability, and financial integrity
  are tested without invented accounting rules.
- Local migration sequence and combined `develop` integration are verified.
- PR documents database, API, security, concurrency, and test impacts.

## 11. Unresolved decisions

Allocation policy, accounting semantics, amount signs, payout provider/method
behavior, authorization matrix, concurrency strategy, reconciliation, status
transitions, and API endpoint shapes require approval.
