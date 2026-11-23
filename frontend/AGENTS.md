# Frontend Instructions (React + TypeScript + Vite)

Applies to `frontend/`. Also follow the root `AGENTS.md`.

## Structure
- Before implementing a business feature, read `../docs/integration/module-contracts.md` and confirm its module owner and API contract.
- Vite + React + TypeScript app under `frontend/`.
- `src/app` — app setup, router, providers, environment config.
- `src/components` — reusable presentational components.
- `src/features/<feature>` — pages and feature-specific components/logic.
- `src/layouts` — shared page shells; `src/services` — API clients (Axios); `src/hooks` — reusable hooks; `src/types` — shared TypeScript types; `src/assets` — static assets.
- Keep UI separate from business logic. Reuse existing components and helpers before adding new ones; avoid excessive abstraction.

## Routing, layouts and auth
- Routing with React Router; public routes and a protected layout for authenticated routes.
- Role-based route protection uses roles from the login response. It improves UX only; the backend remains authoritative.
- Unauthenticated access redirects to `/login`; forbidden roles see the access-denied page.
- Auth state lives in a React context/provider with an in-memory access token (see README for the token-storage decision). No refresh-token flow exists yet.

## API and state
- All backend calls go through the Axios instance in `src/services`. Configure base URL from `VITE_API_URL` and attach the token via an interceptor.
- Centralize error extraction so pages show consistent messages. Handle 401 by clearing auth state.
- Use local component state and the auth context. Do not introduce a global state library without an explicit decision.
- Define shared API types in `src/types`; keep DTO shapes aligned with backend responses.

## UI behavior
- Implement loading, empty, error, and success states where applicable.
- Validate forms before submit and show readable field errors.
- Keep layouts responsive and interfaces accessible (labels, keyboard use, visible focus, meaningful errors).
- Keep the interface simple and professional; business dashboards are not implemented yet.
- SANAD is an Arabic platform. Implement approved UI text and layout in Arabic
  with RTL direction, including shared layouts, navigation, forms, tables,
  validation/error/empty/success states, reports, notifications, and
  accessibility text.
- Reuse shared RTL/layout and component conventions; do not create a separate
  Arabic frontend or duplicate Arabic strings/component implementations.
- Preserve Unicode/Arabic text received from APIs. Do not assume Latin-only
  names, addresses, comments, reports, or identifiers.
- English or additional-language support remains unresolved; do not add
  translations or language selection without approval.

## Testing and validation
- Vitest + React Testing Library for components, login flow, and protected routing.
- Run `npm run build` and `npm test` before finishing UI work. Report what was actually run.
- Do not add npm packages without justification. Do not initialize a second React app.
- Coordinate shared API contract changes with the owning backend module; frontend route or role checks never replace backend authorization.
