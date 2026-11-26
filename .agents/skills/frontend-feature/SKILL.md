---
name: frontend-feature
description: Implement a React + TypeScript + Vite feature in SANAD by inspecting existing components, routing, Axios services, and auth conventions before changing UI. Use when adding or changing pages, components, forms, protected routes, or frontend API integration.
---

# Frontend Feature

## Purpose
Implement a requested UI change consistently with the frontend that exists, without inventing a new architecture or dependency.

## Use when
- Adding or changing a page, component, form, protected route, or frontend API integration.
- Improving loading, error, empty, or success behavior in the React application.

## Inspect first
Read root `AGENTS.md`, `frontend/AGENTS.md`, and `docs/integration/module-contracts.md`. For registration, role-aware UI, or beneficiary/financial data, also read Sections 4–5 of `docs/requirements/requirements.md`. Explore `frontend/src/app`, `features`, `components`, `layouts`, `services`, `hooks`, `types`, and existing tests. Find the closest comparable feature and follow its conventions for routing, Axios usage, auth context, and form validation. Confirm the owning backend module before changing a shared API contract.

## Workflow
1. **Understand the workflow.** Restate the user-facing goal and what is in or out of scope. Ask about unclear behavior instead of guessing business rules.
2. **Identify affected areas.** Name the pages, components, state, and API integration points to change. Reuse existing components and helpers where they fit.
3. **Respect structure and design.** Put feature code under `src/features/<feature>`, shared UI under `src/components`, API calls under `src/services`. Keep UI separate from business logic. Do not introduce a new library or pattern without justification.
4. **Implement states.** Add loading, empty, error, and success states where applicable. Validate forms and show readable errors. Keep layouts responsive and accessible. Frontend permission checks are UX only; the backend is authoritative.
   Check every visible action and data field against the approved role scope.
   Do not expose unresolved or unapproved data in the UI.
5. **Preserve Arabic RTL.** SANAD is an Arabic platform. Use Arabic text and RTL layout for approved UI, including navigation, forms, tables, validation, reports, notifications, and accessibility text. Preserve Unicode API data and reuse shared RTL/layout components. Do not invent English or additional-language support.
6. **Keep it scoped.** Do not initialize another app, add npm packages, or change unrelated UI. Avoid excessive component abstraction.
7. **Validate.** Run `npm run build` and `npm test` (including new tests for login/routing behavior when relevant). Report exactly what ran and what did not.

## Final response format
- **Files changed:** list paths.
- **Implemented:** brief summary of the user-facing result and decisions made.
- **Validation:** commands run and results, or reason not run.
- **Remaining:** follow-up work, missing decisions, or known concerns.
