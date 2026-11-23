---
name: debugging
description: Diagnose defects in SANAD by reproducing the issue, tracing root cause through code, configuration and logs, and applying the smallest justified fix with validation. Use when a bug is reported, a build or test fails, or behavior is unexpected.
---

# Debugging

## Purpose
Find and fix the actual cause of a problem without rewriting unrelated code.

## Use when
- A bug is reported, a test or build fails, or runtime behavior differs from expectations.

## Workflow
1. **Reproduce or understand the issue.** Collect exact steps, expected vs. actual behavior, environment, and error output. If it cannot be reproduced, say so and work from available evidence.
2. **Inspect relevant code and logs.** Read root `AGENTS.md`, `docs/integration/module-contracts.md`, relevant area `AGENTS.md`, and the code on the failing path. Check configuration, migrations, and recent changes in that area. Verify module ownership before changing shared code and avoid sensitive beneficiary data while investigating.
3. **Find the root cause.** Trace inputs through API, Application, Domain, Infrastructure, and (for UI) Axios/auth layers. Form a hypothesis and confirm it with evidence. Do not patch the visible symptom and stop.
4. **Consider side effects.** Identify other flows that depend on the faulty behavior, especially validation, authorization, workflow state, and financial operations. Check existing tests that rely on current behavior.
5. **Apply the smallest justified fix.** Change only the code responsible for the root cause. Do not reformat, refactor, or rewrite unrelated files. If a broader change is genuinely required, explain and ask first.
6. **Validate.** Re-run the reproduction and the relevant build/tests (backend `dotnet build`/`dotnet test`, frontend `npm run build`/`npm test` as applicable). Confirm the fix works and nearby behavior still passes. If validation is not possible, state that explicitly.
7. **Explain.** Report the root cause, the fix, and exactly what was validated.

## Final response format
- **Issue:** what was observed.
- **Root cause:** evidence-backed explanation with file/line references.
- **Fix:** changes made and why they are minimal.
- **Validation:** reproduction/test results, or reason not run.
- **Follow-up:** any remaining risk or question.
