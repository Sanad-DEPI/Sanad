# GitHub Copilot — Repository Instructions

Follow the nearest `AGENTS.md`: root first, then `backend/AGENTS.md` or `frontend/AGENTS.md`. Those files are the source of truth for the actual .NET 10 Clean Architecture solution and the React + TypeScript + Vite app.

Before business-module work, read `docs/integration/module-contracts.md`; inspect existing entities and never duplicate shared entities. Work on `feature/*` branches, merge the latest `develop` before opening a PR, and target PRs at `develop`. Developers may create feature-branch migrations, but must reconcile snapshots and migration conflicts with the project lead rather than overwriting or deleting another developer's work.

SANAD is an Arabic platform. Frontend work must support Arabic text and RTL
layout and preserve Unicode API data. Do not invent English or additional-
language support or translations.

Backend changes must follow the mandatory Analyze → Domain → Application → Infrastructure → API → Testing → Review order in `backend/AGENTS.md`, with dependency direction and module ownership respected. The `CurrentUser` feature (`/api/me`) is the reference example.

Shared rules live in `.agents/rules/`. Reusable workflows live in `.agents/skills/<name>/SKILL.md`. Path-specific instructions are in `.github/instructions/`.

Keep changes scoped, verify builds/tests, report what was not verified, and never commit secrets or sensitive beneficiary data.
