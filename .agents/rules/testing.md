# Testing Expectations

A realistic baseline for a ~10-day delivery. Test depth should match risk.

## Frameworks and commands
- Backend: xUnit in `backend/tests/SANAD.UnitTests` (domain/application logic) and `backend/tests/SANAD.IntegrationTests` (endpoints, authentication, role authorization via `WebApplicationFactory`). Run `dotnet test backend/SANAD.sln`.
- Frontend: Vitest + React Testing Library in `frontend/`. Run `npm test` in `frontend/`.
- EF Core InMemory is allowed in integration tests only; the applications use SQL Server.

## What to test
- Important business rules: needs scoring, eligibility, allocation, and payout state transitions.
- Validation at boundaries: required fields, invalid formats, rejected states.
- Authorization: users in the wrong role cannot read or modify family, document, or financial data.
- Financial operations: amounts, rounding, retry idempotency, and consistency after partial failures.
- ERD integrity: same-case allocation references, exactly-one ledger source and `EntryType` mapping, notification type/reference validation, append-only audit behavior, and soft-deletion visibility/authorization.
- Edge cases: empty results, concurrent updates, partial failures, and missing related records.
- Base foundation: health endpoint, login, protected endpoints, and role-specific endpoints.

## What not to do
- Do not add tests that only increase coverage without checking behavior.
- Do not assert implementation details that make refactors painful.
- Do not skip tests for changes to authorization, validation, or money movement.

## Practice

- Follow existing test naming and arrangement. Before changing existing behavior, add or run a test capturing current behavior when practical.
- Backend feature developers test against their own local SQL Server after
  applying migrations in the correct order, verify schema relationships and
  constraints, and rerun relevant tests after merging the latest `develop`.
- Run relevant tests before opening a PR and report exactly what was run, what
  passed, and what could not run (for example, no database available).
- Migration integration must be verified against the combined solution before
  the shared development database is updated.
