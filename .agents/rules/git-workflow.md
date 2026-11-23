# Git Workflow

A lightweight workflow for five developers working in parallel over approximately 10 days.

## Branches
- `main` is the stable release branch. Do not commit or push directly to it.
- `develop` is the shared integration branch. Do not commit or push directly to
  it.
- Each developer works on a short-lived `feature/<issue>-<short-name>` branch
  (for example, `feature/12-family-registration`) for their assigned module.
  `fix/`, `docs/`, and `chore/` branches may be used for non-feature work.
- Pull requests target `develop`. The flow is
  `feature/*` → review/approval → `develop` → tested stable release → `main`.

## Commits
- Use concise conventional messages: `feat(scope): summary`, `fix(scope): summary`, `docs: summary`, `chore: summary`, `test: summary`.
- One logical change per commit.
- Check `git status` first and stage files explicitly. Never commit secrets, build outputs (`bin/`, `obj/`, `dist/`, `node_modules/`), local databases, or unrelated edits.

## Synchronization
- Before starting work and before opening a PR: `git fetch origin`, then merge
  the latest `origin/develop` into your feature branch.
- Resolve code, EF migration, and model snapshot conflicts by understanding
  both sides. Never blindly overwrite or accept another developer's snapshot.
- Never delete or modify another developer's migrations without coordination.
- If migrations were created from the same previous snapshot, reconcile them
  against the latest integrated model with the project lead. Do not merge
  conflicting migrations merely to make a PR pass.
- Avoid force-pushing shared branches.
- Ask the other author when intent is unclear; never overwrite their work to
  make the conflict disappear.
- After a PR is merged, delete the feature branch and pull the latest `develop`.

## Pull requests
- Open a PR against `develop`, link the issue, describe changes and testing,
  and list risks.
- Keep PRs focused and request at least one reviewer. Address review comments in new commits.
- Changes touching authorization, sensitive data, or money movement should receive a second reviewer before merge.
- PRs must identify module ownership, Clean Architecture impact, tests, database/migration coordination, API contract changes, and confirm there are no unrelated modifications.

## EF Core migrations
- There is one SQL Server database, one shared `AppDbContext`, one migration
  history, and one shared model snapshot.
- Developers own their module entities, configurations, application logic, APIs,
  tests, and feature-branch migrations when schema changes are required.
- Create and test migrations on the feature branch, keep migration and snapshot
  changes in that branch, and apply them to the developer's local SQL Server.
- The project lead reviews migration operations, snapshot consistency,
  sequential applicability, compatibility with integrated migrations, data
  integrity, security, and privacy before approval.
- After approval, verify the combined migration sequence against the integrated
  solution before updating the shared development database. The project lead
  coordinates this integration; they do not implement another developer's
  Infrastructure code.
