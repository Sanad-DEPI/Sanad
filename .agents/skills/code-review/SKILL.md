---
name: code-review
description: Perform a systematic review of SANAD changes for bugs, Clean Architecture violations, security, authorization, privacy, data integrity, concurrency, financial correctness, performance, maintainability, and test gaps. Use when reviewing a pull request or a diff.
---

# Code Review

## Purpose
Find real defects and risks in a specific change, ordered by severity, with actionable evidence.

## Use when
- Reviewing a PR, a branch diff, or a set of changed files.
- Asked to check code for correctness, security, or financial/business-rule risks.

## Workflow
1. **Define scope.** Read root `AGENTS.md`, `docs/integration/module-contracts.md`, the relevant area `AGENTS.md`, and shared rules. Get the exact diff and the stated intent. Only review what changed and its immediate context.
2. **Understand the change.** Trace the affected flow end to end before commenting. Check module ownership, duplicate entities, and shared migration coordination. For backend changes, check layer placement against `backend/AGENTS.md` and the `CurrentUser` reference feature.
3. **Check each applicable area:**
   - Bugs: incorrect logic, null/empty handling, wrong status or error paths.
   - Architecture: dependency direction, business rules in controllers/Infrastructure, unnecessary abstractions, module ownership.
   - Security and authorization: server-side enforcement, role checks, JWT handling, data exposure.
   - Privacy: beneficiary data in logs, APIs, files, or error messages.
   - Business rules: eligibility, scoring, allocation, and workflow state transitions.
   - Data integrity and concurrency: transactions, concurrent updates, idempotency, partial failures, migrations, and model snapshot consistency.
   - ERD integrity: Identity key types, same-case allocation references, ledger source constraints, notification type references, and selective soft deletion.
   - Migration integration: sequential applicability, compatibility with
     previously integrated migrations, conflict reconciliation, and no
     blind snapshot or migration overwrites.
   - Financial correctness: amounts, currency, rounding, reconciliation, payout authorization.
   - Performance: avoidable N+1 queries, unbounded queries, blocking work on request paths.
   - Maintainability and tests: duplicated behavior, unclear names, missing tests for changed rules/auth/validation/money movement.
4. **Verify before reporting.** Every finding must point to a specific file, line, and concrete scenario. If a concern depends on an assumption you cannot confirm, label it as a question, not a defect. Do not invent hypothetical problems as if they exist in the code.
5. **Assign severity:** `Critical` (money/data loss or security breach), `High` (likely broken behavior or unauthorized access), `Medium` (edge case or maintainability risk), `Low` (minor polish). Report only actionable findings; say explicitly if none were found.

## Final response format
- **Scope reviewed:** files/PR and intent.
- **Findings:** severity, file/line, what is wrong, why it matters, suggested fix.
- **Questions:** unconfirmed assumptions needing author input.
- **Overall:** approve / request changes / needs clarification.
