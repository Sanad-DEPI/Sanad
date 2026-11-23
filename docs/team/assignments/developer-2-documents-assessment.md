# Developer 2 — Document Verification & Assessment

## 1. Purpose and responsibilities

Own document management and verification, needs assessment, scoring, priority
calculation, and specific-need records. Detailed scoring formulas, verification
rules, and acceptance criteria remain pending.

## 2. Entity ownership and ERD relationships

Own exactly:

- `DOCUMENT`
- `SCORINGRULESET`
- `SCORERESULT`
- `SPECIFICNEED`

`DOCUMENT` references Module 1 `CASE` and the Identity user who uploads it.
`SCORERESULT` references `CASE` and `SCORINGRULESET`. `SPECIFICNEED`
references `CASE` and the Identity user who values it. Module 4 may reference
`SPECIFICNEED` from `DONATIONALLOCATION.NeedId`; the need must belong to the
same case as the allocation. Follow the ERD for fields, keys, statuses, and
selective soft deletion.

## 3. Complete user-facing workflows

The approved requirements establish document management and verification, needs
assessment, scoring, priority, and specific needs. The ERD establishes scoring
rule versions/results and need statuses, but not the scoring algorithm,
verification policy, authorization matrix, or state transitions. Do not invent
these rules.

Deliver complete vertical workflows from the approved requirements: backend
validation, authorization, persistence, and APIs must work with the relevant
frontend pages, forms, API calls, and user feedback. Do not deliver isolated
APIs or frontend mockups. Document unclear workflows and request approval.

## 4. Backend deliverables and API surface

Implement only the required Domain entities/rules, Application use cases,
services, DTOs and validation, Infrastructure configurations and persistence,
API endpoints and authorization, and focused backend tests. No business-module
endpoint paths are approved. Propose endpoints only with an approved use case
and follow the existing API conventions. Do not expose encrypted personal data,
file contents, scoring internals, or sensitive values without an approved
contract.

## 5. Frontend deliverables

Implement the approved document, assessment, scoring, and specific-need pages
and routes, module-specific components and forms, Axios integration, role-aware
UI access, and loading, empty, error, success, and validation states. Use
Arabic text and RTL layout and preserve Unicode document metadata and names. Reuse the
existing React layouts, navigation, auth context, shared components, and types.
Frontend checks are UX only; backend authorization is authoritative.

## 6. Shared frontend integration and dependencies

Module 2 depends on Module 1 `CASE`. Coordinate case identifiers and lifecycle
assumptions with Developer 1. Coordinate `SPECIFICNEED` identifiers, case
ownership validation, and status contracts with Developer 4. Use string Identity
foreign keys and GUID business keys; do not duplicate Identity or `CASE`.

Consume other modules through agreed APIs and identifiers. Coordinate changes to
shared frontend files with the project lead, and use actual agreed APIs for
final integration rather than temporary mocks. Do not duplicate another
module's backend logic or entities.

## 7. Required backend and frontend tests

Test approved document validation/authorization, case references, file metadata
rules, scoring-rule version behavior, score-result relationships, specific-need
same-case validation when consumed by Module 4, soft deletion, and sensitive
data handling. Add frontend component/integration tests for approved pages,
forms, routes, API states, and role-aware UX. Apply local migrations and test
after integrating the latest `develop`.

## 8. Mandatory backend implementation order

Follow **Analyze → Domain → Application → Infrastructure → API → Testing →
Review** and the shared [backend feature skill](../../../.agents/skills/backend-feature/SKILL.md).
Only implement layers required by an approved feature.

## 9. Git and EF Core migration workflow

Use a `feature/*` branch. Create and test required migrations there, apply them
sequentially to local SQL Server, merge the latest `develop` before the PR,
reconcile migration/model-snapshot conflicts with the project lead, and target
the PR at `develop`. Never push directly to `develop` or `main`.

## 10. Definition of Done

- Owned entities and configurations match the approved ERD.
- `CASE` and `SPECIFICNEED` cross-module contracts are coordinated.
- Required backend and frontend workflows are integrated end to end.
- Frontend uses shared conventions and actual agreed APIs.
- No invented scoring, verification, or status rules are merged.
- Validation, authorization, privacy, and tests are complete.
- Migrations and local schema apply correctly after `develop` integration.
- PR review identifies API, database, and contract impacts.

## 11. Unresolved decisions

Document storage, verification workflow, scoring formula, authorization,
specific-need lifecycle, retention, and API endpoint shapes require approval.
