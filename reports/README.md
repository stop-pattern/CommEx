# Work records and verification reports

Create `reports/<work-id>/` for every sustained task. Use the feature ID/name for feature work
(for example F004-udp-transmission); descriptive IDs identify documentation/infrastructure work.
Follow [AGENTS.md](../AGENTS.md) and the [development workflow](../docs/development-workflow.md).

## Persistent files

| File | Required content | Handling |
|---|---|---|
| goal.md | Goal, authorized scope, acceptance, dependencies and constraints | Keep current; link approved spec/plan/tasks |
| progress.md | State, active task, branch/commit/diff, attempts/limit, hypotheses, evidence, ownership, deployed resources, blockers, next exact action | Rewrite as a coherent resume snapshot at each meaningful checkpoint |
| journal.md | Timestamped actions, findings, hypotheses/experiments, decisions, commands/exit codes, evidence and commits | Append historical events; record corrections explicitly |
| summary.md | Outcome, acceptance evidence, remaining work, limitations and decisions | Required at completion or blocking |
| result.json | Terminal feature verification result conforming to result.schema.json | Feature verification only; do not invent fields/statuses for progress |

Start from the [goal](./_templates/goal.md), [progress](./_templates/progress.md) and
[journal](./_templates/journal.md) templates. Replace all placeholders for actual work; the templates
themselves are not active tasks or claims of verification.

The [result schema](result.schema.json) supports passed/blocked/failed, not running/checkpoint states.
Keep active state in progress.md. Documentation-only audits may use a clearly labeled separate JSON
record; never make them look like a passing plugin feature.

## Checkpoint timing and resumption

Record necessary detail during the task, not only after success. Update before/after long commands,
build/test/deployment runs, plan changes, new findings, failures, delegation/integration, commits,
blocks, handoffs and turn/context boundaries. Save pending operations before executing them.
On resume verify actual process/deployment/Git state before concluding an interrupted action succeeded
or before repeating it. Use atomic/complete snapshot writes to avoid partial resume state.

Carry the feature attempt count and configured limit across restarts/sessions/subagents. Keep an event
record for each hypothesis and verification cycle. At the limit, record BLOCKED and the smallest required
decision/environment change. Do not reset or relabel attempts to avoid bounded autonomy.

## Evidence and privacy

Each verification event identifies time, command with safe arguments, exit code, tool/host versions,
tested commit and dirty-tree state, relevant artifact hash, observations and relative evidence paths.
Record failures and rejected hypotheses as well as successes. Link resource ownership and any deployed
artifact so another agent can safely resume or clean up.

Commit only sanitized records. Do not include usernames in absolute paths, private identity/contact
data, host identifiers, secrets, private keys, credential-bearing command lines or confidential payloads.
Use repository-relative paths, environment-variable placeholders and sanitized aliases for references.
Raw evidence belongs in ignored logs/, screenshots/, test-results/ or artifacts/; being ignored does
not make raw data safe to publish. Review filenames, generated output and screenshots as well as text.
Never paste a discovered secret into an issue, journal or conversation.

## History, consistency and commits

Keep progress.md current; journals and commit-bound results preserve historical facts. Do not rewrite
old reports to imply that a later implementation was already tested. New corrections must identify the
prior evidence and reason. Normative documentation should instead be rewritten coherently whenever rules change.

Commit each smallest meaningful checked unit, independently of prompt boundaries. Keep related
implementation/tests and mutually dependent documents together; retain verification records as a
separate evidence increment when needed. Coordinator owns shared-tree staging/commits and integrates
subagent evidence. Record commit IDs in the next checkpoint; do not create endless commits only to
write a report's own final commit ID into itself.

A passing feature report requires all applicable acceptance and scripts/verify.ps1 to pass with no
required stage skipped. A saved checkpoint, a completed subagent or a documentation check is not a
feature completion signal.
