# Developer 1 — Identity & Case Management

## 1. Purpose and responsibilities

Own family registration and case management: families, family members, case
creation, case surveys, identity-validation-related case work, lifecycle
coordination, and immutable status history. Detailed business rules and
acceptance criteria remain pending in the requirements document.

Role boundary: Beneficiaries register only their own family and track only
their own cases. Each Beneficiary Identity user owns exactly one family through
the required unique `FAMILY.PrimaryBeneficiaryUserId` string FK to the existing
Identity user; each family has exactly one primary Beneficiary. Field Officers
do not perform family registration and must receive only non-identifying
assessment information. Only Manager may deactivate/reactivate a family
account, with a mandatory deactivation reason. Implement no role or data
permission beyond Sections 4–5 of the requirements.

## 2. Entity ownership and ERD relationships

Own exactly:

- `FAMILY`
- `FAMILYMEMBER`
- `CASE`
- `CASESURVEY`
- `CASESTATUSHISTORY`

Key relationships include family-to-members, family-to-cases, case-to-survey
(one-to-one), case-to-status-history, and Identity-user references for case
creation and status review. `FAMILY.PrimaryBeneficiaryUserId` references the
existing Identity user ID, not a duplicate user entity. `FAMILYMEMBER` remains
owned by its family. `CASE` is consumed by Modules 2, 4, and 5.
`CASESTATUSHISTORY` is immutable historical data. Follow the approved ERD for
fields, keys, cardinalities, and selective soft deletion.

## 3. Complete user-facing workflows

The approved requirements establish family registration, family members, case
management, identity validation, case surveys, and case lifecycle scope. The
exact workflows, status-transition authorization, validation rules, and
acceptance criteria are unresolved; do not invent them.

Deliver complete vertical workflows from the approved requirements: the
backend API, validation, authorization, and persistence must work with the
corresponding frontend pages, forms, navigation, API calls, and user feedback.
Do not deliver isolated APIs or frontend mockups. If a workflow or integration
is unclear, document the limitation and request approval.

## 4. Backend deliverables and API surface

Implement only the required Domain entities/rules, Application use cases,
services, DTOs and validation, Infrastructure configurations and persistence,
API endpoints and authorization, and focused backend tests. No business-module
endpoint paths are approved yet. Define endpoints only after the use cases and
contracts are approved, following the existing controller, DTO, validation,
`ApiResponse<T>`, authorization, and error-handling conventions.

## 5. Frontend deliverables

Implement the required case-management pages and routes, module-specific
components and forms, Axios API integration, role-aware UI access, and loading,
empty, error, success, and validation states. Use Arabic text and RTL layout
across the workflow, and preserve Unicode names and comments. Use the existing React structure,
layouts, navigation, auth context, shared components, and TypeScript types.
Frontend checks are UX only; backend authorization is authoritative.

## 6. Shared frontend integration and dependencies

Use existing Identity user IDs as string values; do not create authentication
entities. Coordinate the `CASE` contract with Developer 2 for documents,
scoring, and needs, and with Developer 4 for allocations and payouts. Developer
5 consumes case-status information for notifications and audit events.

Consume other modules through agreed APIs and identifiers; do not duplicate
their backend logic, entities, or authentication. Coordinate any changes to
shared layouts, navigation, authentication integration, components, or global
types with the project lead. Final cross-module integration must use actual
agreed APIs rather than temporary mocks.

## 7. Required backend and frontend tests

Add focused backend unit and integration tests for approved family/case rules,
validation, authorization, case-survey cardinality, status-history immutability,
soft-deletion visibility, and cross-module case references. Add frontend
component/integration tests for the approved pages, forms, routes, API states,
and role-aware UX. Test with local SQL Server migrations and the integrated
`develop` branch as required by the shared workflow.

## 8. Mandatory backend implementation order

Follow **Analyze → Domain → Application → Infrastructure → API → Testing →
Review**. Only create layers and classes required by the approved feature.
Follow [backend instructions](../../../backend/AGENTS.md) and the
[backend feature skill](../../../.agents/skills/backend-feature/SKILL.md).

## 9. Git and EF Core migration workflow

Work on `feature/*`, create required migrations on that branch, apply them in
order to your local SQL Server, merge the latest `develop` before opening the
PR, and reconcile code/migration/snapshot conflicts with the project lead. Open
the PR against `develop`; never push directly to `develop` or `main`.

## 10. Definition of Done

- Approved requirements and module contract are satisfied.
- Owned entities/configurations follow the ERD and Identity key rules.
- Required backend and frontend workflows are integrated end to end.
- Frontend uses shared conventions and actual agreed APIs.
- Cross-module `CASE` contracts are documented and coordinated.
- Validation, authorization, privacy, and historical behavior are covered.
- Local migrations apply sequentially and the local database is verified.
- Relevant unit/integration tests pass before and after merging `develop`.
- PR targets `develop` and contains no unrelated changes.

## 11. Unresolved decisions

Detailed identity-validation rules, case status transitions, authorization
matrix, retention requirements, and API endpoint shapes require approval.
