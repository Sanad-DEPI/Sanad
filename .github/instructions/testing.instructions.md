---
applyTo: "**/*test*,**/*spec*,**/__tests__/**"
---

# Tests (Copilot)

Source of truth: `.agents/rules/testing.md` and root `AGENTS.md`. Follow them in full.

Critical checks:
- Read `docs/integration/module-contracts.md` to verify module ownership and shared-contract impact before adding feature tests.
- Backend feature testing must use the developer's local SQL Server with migrations applied in order, verify schema relationships/constraints, run relevant unit and integration tests, and be repeated after merging the latest `develop`.
- Backend: xUnit; unit tests for rules/services, integration tests with `WebApplicationFactory` for endpoints, auth, and role authorization. Run `dotnet test backend/SANAD.sln`.
- Frontend: Vitest + React Testing Library for login and protected routing. Run `npm test` in `frontend/`.
- Cover business rules, validation, authorization, financial operations, and edge cases. No coverage-only tests.
- Validate role-specific permissions against Sections 4–5 of
  `docs/requirements/requirements.md`, including denial of unresolved access;
  do not use UI visibility as evidence of authorization.
- Report exactly what ran, what passed, and what could not run.
