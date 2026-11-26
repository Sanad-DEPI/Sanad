# Project Overview

## What SANAD is

SANAD (SACMP — Smart Aid & Charity Management Platform) is a charity management platform intended to support family registration and case management, identity validation, document management and verification, needs assessment and scoring, donations and payment processing, fund allocation, payout authorization and execution, notifications, reporting, and audit trails.

## Agreed technology

- Backend: .NET 10 (LTS) ASP.NET Core Web API using Clean Architecture.
- Frontend: React + TypeScript + Vite, Axios, React Router.
- Database: SQL Server with EF Core; ASP.NET Core Identity + JWT.
- Repository: monorepo with two independently runnable applications.

## Scope and constraints

- Team of five developers, approximately 10-day timeline.
- SANAD is an Arabic platform. Approved user interfaces must support Arabic
  text and RTL layout, and backend/API text fields must preserve Unicode
  content. English or additional-language support remains an unresolved
  localization decision.
- Detailed requirements for the business modules are not yet defined; do not invent business rules.
- No hosting provider or availability assumption is treated as a constraint.
- The five business modules and their tables are intentionally not implemented.
- The proposed module ownership and integration rules are documented in
  [`integration/module-contracts.md`](integration/module-contracts.md). The
  ERD at [`architecture/sanad_erd.mermaid`](architecture/sanad_erd.mermaid) is
  the source for the verified entity model; remaining design questions are
  documented for team approval.

## Important principles

- Security and privacy for sensitive beneficiary data.
- Server-side authorization and validation; frontend checks are not security.
- Approved account types, role responsibilities, and data-access boundaries
  are defined in Sections 4–5 of
  [`requirements/requirements.md`](requirements/requirements.md). Treat that
  document as authoritative; do not infer role permissions from the current
  authentication foundation or ERD.
- Separation of duties and financial consistency for allocation and payout operations.
- Auditability and maintainability.
- Implement only agreed requirements; keep solutions simple.

## Implemented foundation

- Backend solution with Domain, Application, Infrastructure, API, UnitTests, and IntegrationTests projects.
- Identity + JWT authentication, six legacy seeded roles, and an optional
  development-only Manager account. These are current implementation facts,
  not the approved target account/role plan.
- `/health`, `/api/auth/login`, `/api/me`, `/api/test/authenticated`, and Manager-only `/api/test/manager`.
- EF Core SQL Server configuration and initial Identity migration.
- Consistent API response envelope, validation mapping, exception handling, CORS, Swagger, and health checks.
- React frontend with login, protected routes, role guard, dashboard, Manager page, logout, access-denied state, and API error handling.
- Unit/integration tests on the backend and Vitest/React Testing Library tests on the frontend.
- Planned modules share one application, one `AppDbContext`, one SQL Server
  database, and one coordinated migration history; the project lead coordinates
  reviewed migration integration. Development uses `develop` as the shared
  integration branch and `main` as the stable release branch.

## Not implemented

Family/case management, document verification workflows, needs assessment and scoring, donations/payments, fund allocation and payouts, notifications/reporting/audit modules, payment gateways, refresh tokens, and microservices.

## Status

The base foundation is implemented and verified locally. Its current six-role
seed and development Manager bootstrap predate the approved account/role plan;
they are implementation status only and are not the target registration or
permission model. Configuration, run instructions, exact verification commands, and the token-storage decision are in `README.md`; the implemented architecture is described in `docs/architecture/foundation.md`.

The ERD confirms the five-module ownership allocation. Its approved Identity
key decision, integrity constraints, and remaining design questions are
documented in
[`integration/module-contracts.md`](integration/module-contracts.md) and require
approval before implementation.
