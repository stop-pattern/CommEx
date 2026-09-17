# Repository instructions

## Goal

Implement the approved BveEX communication plugin specifications autonomously and leave
machine-verifiable evidence. A plausible implementation is not completion; verification is.

## Authority and source order

When instructions conflict, use this order:

1. Approved feature specification and acceptance criteria under `specs/`
2. `.specify/memory/constitution.md`
3. This file
4. Feature plan and task list
5. Existing implementation conventions

Do not silently resolve a product-level ambiguity. Record it as BLOCKED when it changes the
external protocol, safety behavior, compatibility, persistence format, or user-visible behavior.

## Non-negotiable constraints

- Target .NET Framework 4.8 unless a specification explicitly approved by the human says otherwise.
- Use WinForms for the settings UI.
- Do not block a BveEX/BVE callback thread with network, serial, process, or file I/O.
- Make lifetime and shutdown explicit. Stop workers, cancel pending operations, detach events,
  close ports/sockets, and release UI resources during disposal.
- Keep protocol encoding/decoding independent from BveEX and UI code.
- Validate configuration before applying it. Never overwrite a known-good configuration with
  invalid or partially parsed data.
- Do not edit or install files outside paths declared in `config/repo.local.json`.
- Never declare a feature complete while any required verification level is skipped or failing.

## Required workflow for each feature

1. Read the feature spec, plan, tasks, and acceptance criteria.
2. Split work into the smallest testable increments; update `tasks.md` when the plan changes.
3. Add or update tests before or alongside implementation.
4. Implement the smallest coherent increment.
5. Run the narrowest relevant tests, then `scripts/verify.ps1`.
6. On failure, preserve evidence, form a falsifiable cause hypothesis, and test it.
7. Commit each coherent increment after its applicable checks pass, following the commit policy below;
   do not wait for the entire feature or session to finish.
8. After full success, write `reports/<feature-id>/summary.md` and `result.json`.
9. Mark tasks complete only after their acceptance evidence exists.

Maximum autonomous attempts for one feature are configured in
`config/repo.local.json`. After the limit, mark the feature BLOCKED and continue only with
features that do not depend on it.

## Commands

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-unit.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-integration.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-bve.ps1
powershell -ExecutionPolicy Bypass -File scripts/verify.ps1
```

Do not substitute ad-hoc build commands for the repository scripts when reporting completion.

## Change discipline

- Preserve unrelated user changes.
- Do not change the target framework, public protocol, configuration schema, or dependency set
  merely to make a test pass.
- Do not weaken, delete, skip, or rewrite a failing test unless the approved specification proves
  that the test is wrong. Explain such a change in the report.
- Avoid broad refactors during a feature unless required for correctness.
- Never commit secrets, machine-local paths, BVE distribution files, or third-party binaries.
- A commit must represent one smallest meaningful, reviewable increment: a specification/documentation
  update, an implementation with its related tests, a focused fix, or a scoped infrastructure change.

## Commit policy for agents

- Commit frequently at meaningful boundaries. Once a coherent increment has passed its applicable
  checks, commit it before moving to the next independent change. Do not accumulate unrelated work
  into a single end-of-session commit or wait for a whole feature to be complete.
- Group changes by purpose, not by file count. Keep an implementation and its corresponding tests,
  or documents that must change together to stay consistent, in the same commit. Avoid fragments
  that are not independently understandable or leave an intentionally broken intermediate state.
- Before every commit, inspect `git status`, the diff, and the staged diff; run `git diff --check`
  and the checks relevant to that increment. Stage explicit related paths or hunks, preserving
  unrelated user changes. Do not use blanket staging to include work you did not review.
- For documentation-only changes, check content consistency, references/links and whitespace.
  A pre-existing missing solution or unavailable runtime does not prevent committing verified
  documentation; record the limitation without claiming build or feature success.
- For code changes, pass the increment's applicable tests and retain verification evidence. If a
  broader feature gate is still unavailable or failing outside the increment, record that fact;
  the commit is an intermediate increment, not a completed feature. Do not commit an increment
  whose own applicable checks fail merely to increase commit frequency.
- Use commit messages that identify the concrete purpose. Report created commit IDs in the final
  response. The owner's standing instruction authorizes these local incremental commits; no
  additional confirmation is needed unless the current request explicitly restricts commits.
- A commit is not a completion signal. Feature completion still requires every applicable acceptance
  criterion and `scripts/verify.ps1` to pass without skipped required stages. Record the tested commit
  in the feature evidence; an evidence-only follow-up commit is a separate meaningful unit.

## Definition of done

A feature is done only when all applicable acceptance criteria are satisfied, `verify.ps1` exits 0,
no required stage was skipped, the working tree contains no unintended changes, and the report
identifies the tested commit and retained evidence.
