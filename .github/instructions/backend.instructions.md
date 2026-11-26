---
applyTo: "backend/**"
---

# Backend scope (Copilot)

Source of truth: `backend/AGENTS.md` and root `AGENTS.md`. Follow them in full.

Critical checks for backend changes:
- Read `docs/integration/module-contracts.md` before feature work; verify ownership and inspect existing entities before creating anything.
- Respect dependency direction: Domain → nothing; Application → Domain; Infrastructure → Application + Domain; API → Application + Infrastructure for composition only.
- Follow the mandatory order: Analyze → Domain → Application → Infrastructure → API → Testing → Review. Only create classes in layers the feature needs.
- Keep business rules out of controllers/Infrastructure; use DataAnnotations validation, `ApiResponse<T>`, DI, centralized exception handling, EF Core migrations, and Identity/JWT roles from `AppRoles` for the current foundation. Target role names and permissions follow the requirements document.
- Check module ownership before editing another module or shared contract.
- Before changing registration, role assignment, authorization, or beneficiary/
  financial data, use Sections 4–5 of `docs/requirements/requirements.md` as
  the authority. Enforce least privilege per operation and response field;
  deny unresolved access and never invent role permissions.
- Create required migrations on the feature branch, apply them to the developer's local SQL Server, and coordinate snapshot/migration conflicts with the project lead. Never overwrite or delete another developer's migrations; PRs target `develop` after merging the latest `develop`.
- Preserve the documented ERD integrity rules: string Identity keys, GUID business keys, same-case allocations, exactly-one ledger source, type-specific notification references, generic append-only audit logs, and selective soft deletion.
- Build and run tests; report exactly what was verified.
