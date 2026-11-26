---
applyTo: "frontend/**"
---

# Frontend scope (Copilot)

Source of truth: `frontend/AGENTS.md` and root `AGENTS.md`. Follow them in full.

Critical checks for frontend changes:
- Read `docs/integration/module-contracts.md` before business-feature work and coordinate API contract changes with the owning backend module.
- Structure code under `src/app`, `components`, `features`, `layouts`, `services`, `hooks`, `types`, `assets`.
- Use the shared Axios service with `VITE_API_URL` and auth interceptor; use React Router with protected/role-aware routes and the auth context.
- Handle loading, empty, error, success, and form-validation states; keep UI accessible and responsive. Frontend role checks are not security enforcement.
- Check every role-specific action and data field against Sections 4–5 of
  `docs/requirements/requirements.md`. Do not expose unapproved or unresolved
  data in the UI; backend authorization remains authoritative.
- SANAD is Arabic-first. Approved UI must support Arabic text and RTL layout,
  and API data must preserve Unicode text. Do not invent additional-language
  support or translations.
- Run `npm run build` and `npm test`; report what was verified.
