# Repository instructions

## Goal and authority

Develop the approved CommEx AtsEX/BveEX communication plugin autonomously, preserving resumable
context and machine-verifiable evidence. A plausible implementation is not completion; verification is.

When project documents conflict, use this order:

1. Approved feature specification and acceptance criteria under specs/.
2. .specify/memory/constitution.md.
3. This file and the operational rules it references.
4. Feature plan and task list.
5. Existing implementation conventions.

The user's current instructions govern the authorized scope. Do not silently decide product-level
ambiguities affecting protocols, safety, compatibility, persistence or user-visible behavior.
Record the affected work as BLOCKED, with evidence and the smallest required decision. Continue
independent authorized work. The [development workflow](docs/development-workflow.md) expands these rules.

## Non-negotiable constraints

- Target .NET Framework 4.8 unless an explicitly approved specification says otherwise; use WinForms.
- Never block a BVE/AtsEX/BveEX callback with network, Serial, process or file I/O.
- Keep codecs independent from hosts, UI and transport I/O. Bound queues and resource use.
- Validate the full configuration before applying it. Invalid/partially parsed input must not overwrite
  known-good data. Follow the approved distinction between invalid settings and an unavailable peer.
- Make shutdown explicit: stop workers, cancel operations, detach events, close sockets/ports and release UI.
- External writes/installations are restricted to paths declared in config/repo.local.json and authorized
  for the task. Do not infer a target process, device or deployment destination.
- Preserve unrelated changes and never weaken tests, targets, protocol/schema or dependencies to make a check pass.
- Never claim feature completion when required verification is failing, unavailable or skipped.

## Autonomous development cycle

1. Read the goal, checkpoint, recent journal/evidence, approved spec, plan, tasks and acceptance criteria.
   Reconcile them with actual branch/HEAD/diffs, active agents and deployed resources.
2. Record a bounded goal in `reports/<work-id>/goal.md`. Split it into the smallest testable increments
   and update the task plan before implementing changes to that plan.
3. Delegate independent research/review and separable implementation work as described below.
4. Add/update relevant tests before or alongside the smallest coherent implementation.
5. Build and run the narrowest applicable tests using repository scripts. Persist command, exit code,
   revision, timestamps and evidence references immediately.
6. On failure preserve evidence, reproduce, form a falsifiable hypothesis, perform a discriminating
   experiment, repair the cause, then rebuild/retest. Record rejected hypotheses; do not blindly repeat.
7. When integration requires deployment, verify the artifact and authorized target, coordinate resource
   ownership, stop/unload safely, preserve recovery material, deploy and verify what was loaded.
8. Debug through logs, communication and observable host behavior. Iterate analysis/fix/build/deploy/debug
   within the configured attempt budget, then run scripts/verify.ps1 and all applicable acceptance checks.
9. Commit each meaningful increment after its applicable checks pass. Update checkpoint/evidence before
   beginning independent work. Prompt or turn boundaries do not determine commit boundaries.
10. Complete a feature only after all required gates pass. Write summary.md and schema-valid result.json,
    identify the tested commit and retained evidence, and then mark eligible tasks complete.

Use the configured maximumFeatureAttempts across sessions, agents and restarts; persist attempts in
progress.md and never reset them to evade the limit. After exhaustion record BLOCKED and its recovery
condition. Continue only independent work. Do not start an unapproved feature to stay busy.

## Durable context and resumption

Use `reports/<work-id>/` as the durable record. Feature IDs identify feature work; descriptive IDs may
identify documentation/infrastructure work. See [report rules and templates](reports/README.md).

- goal.md: objective, authorized scope, acceptance, dependencies and constraints.
- progress.md: current task/state, branch and revision, relevant working-tree changes, attempts/budget,
  evidence, active agent ownership, deployment/resource state, blockers and next exact action.
- journal.md: dated actions, discoveries, decisions, hypotheses, experiments and their outcomes.
- summary.md and feature result.json: final verified outcome or blocked/failed result, never fabricated progress.

Write necessary fine-grained progress throughout the task: before/after long commands and deployments,
on plan changes, discoveries, failures, delegation/results, integration, commits, blocks and handoffs;
also before ending a turn or losing context. Do not rely on chat or agent memory alone. Record facts,
rationale and evidence rather than private reasoning transcripts or redundant output dumps.

Keep progress.md a coherent current snapshot and journal.md a historical action record. Save pending
operations before starting them; reconcile unfinished operations with actual state on resume.
Do not assume an interrupted command succeeded, rerun a deployment blindly or overwrite another agent's work.
Retain failed-attempt evidence before further changes. A session ending does not itself keep work running.

## Parallel agents and shared resources

- Proactively delegate independent read-only analysis, research, reference checks and reviews when useful.
- Also parallelize implementation, tests and documentation when file/resource ownership can be separated.
  For tightly coupled work, keep one owner or serialize rather than creating competing writers.
- Supply each subagent a concrete goal, scope, relevant context/sources, allowed files, exclusions,
  expected deliverables and verification. Persist assignments, status and returned evidence in progress.md.
- In a shared worktree, assign nonoverlapping write scopes. The coordinator owns the Git index, commits
  and integration; subagents must not stage/commit unless explicitly given exclusive ownership.
- Serialize deployment and use of shared BVE processes, UI automation, COM/listening ports and fixtures.
  Separate worktrees do not automatically isolate those resources.
- Review agent output and diffs, reconcile conflicts, run required integration checks, then update status.
  A subagent's success claim is not verification. Stop/reassign stale work when scope changes and record it.

## Repository layout

The following tree is the canonical responsibility layout and must match the README tree. Entries
marked planned are not claims of existing code. Do not scatter implementation, tools or evidence at
the repository root. Exact project/assembly names and dependencies remain feature-plan decisions.

```text
CommEx/
|-- AGENTS.md                 # mandatory agent instructions
|-- README.md                 # human entry point
|-- .editorconfig             # shared C# formatting and naming preferences
|-- docs/                     # development and coding guides
|-- specs/                    # product and approved feature contracts
|-- .specify/                 # constitution, Spec Kit templates/scripts
|-- .agents/                  # repository-local agent skills
|-- config/                   # tracked examples; ignored machine-local configuration
|-- scripts/                  # build, test, deploy and orchestration entry points
|-- src/CommEx/               # net48 BveEX bootstrap; communication features planned
|-- tests/
|   |-- Unit/                 # planned: deterministic tests
|   |-- Integration/          # bootstrap loader contract; transport tests planned
|   `-- BveE2E/               # configured-host bootstrap assertions; six-host tests planned
|-- tools/TestPeer/           # planned: independent external test peer
|-- reports/
|   |-- _templates/           # sanitized goal/progress/journal templates
|   `-- <work-id>/            # resumable records and verification reports
`-- artifacts/               # ignored build/debug/raw evidence
```

- Put production code under `src/<project>/`, organized by host adapter, core, codec/communication,
  configuration, UI and diagnostics responsibilities. Logical folders do not require separate DLLs.
- Put unit, integration and host E2E tests under the corresponding tests/ responsibility folder.
  Independent executable test peers belong under tools/TestPeer/.
- Keep feature contracts in specs/, reusable development guidance in docs/, commands in scripts/,
  sanitized work records in reports/, and generated/raw outputs in ignored artifacts/ or report evidence folders.
- Keep .specify/ and .agents/ for workflow configuration/skills, not application source or raw evidence.
  Existing root research material remains a labeled source referenced by the normative specs.
- Introduce new top-level areas only for a clear responsibility and update this tree and README together.
  Machine-local config, third-party binaries and generated build outputs must not become tracked sources.

## C# coding conventions

Follow Microsoft's C# conventions within the approved compiler and .NET Framework 4.8 capabilities;
do not adopt unsupported language features or upgrade dependencies/frameworks for style alone.
Apply the [C# coding standards](docs/coding-standards.md) and shared [.editorconfig](.editorconfig).
Editor settings do not replace review of XML documentation, especially for non-public declarations.
Repository overrides are mandatory:

- Use four spaces per indentation level; no tabs.
- Write XML documentation comments (///) for every handwritten type and member, including non-public
  classes, interfaces, methods/functions, constructors, properties, events and fields.
- Provide a meaningful summary and applicable parameter, type-parameter, return/value and exception
  documentation. Update it when behavior changes. Explain contracts and constraints, not just names.
- Generated code is governed through its generator/template; document handwritten partials and record
  limitations. Compiler documentation warnings alone do not prove coverage of non-public declarations.

## Documentation consistency

When information is added, removed or changed, find and update affected specifications, plans, tasks,
acceptance criteria, examples, README, agent instructions and guides in the same coherent increment.
Rewrite the relevant narrative/tables as if the current rule had always belonged there. Remove superseded
normative statements and dangling references; do not leave contradictory append-only amendments.

Prefer one authoritative description with links. Required duplicated content, including the folder
tree here and in README, must stay synchronized. Historical journals, decisions and commit-bound
verification reports preserve what was true then; label corrections rather than falsifying past evidence.

## Privacy and commit discipline

- Never commit personal/confidential information: usernames in absolute paths, private names/emails,
  identifying host details, credentials, tokens, private keys, connection secrets or confidential data.
  Use repository-relative paths, environment-variable placeholders and sanitized examples.
- Check filenames, contents, command arguments, stack traces, screenshots, generated reports, commit
  messages and Git author/committer identity. Use a publicly authorized or anonymous Git identity;
  do not silently expose a local identity or change global Git settings.
- Keep raw machine-specific evidence in ignored locations; commit sanitized summaries/references only.
  Ignoring a file is not secret scanning and does not remove already tracked sensitive data.
  Do not echo discovered secrets. Report exposure and remediation without unauthorized history rewriting.
- Commit the smallest meaningful, reviewable increment as soon as its applicable checks pass, before
  moving to independent work. One prompt may produce multiple commits; a session is not a commit unit.
- Group by purpose, not file count. Keep implementation/tests or interdependent documentation together;
  avoid broken fragments and unrelated changes. Do not commit failing applicable checks just for frequency.
- Keep commits separate by task. Before editing or staging, reconcile existing task records, owners and
  working-tree changes; coordinate overlapping files with their owners. Include only the current task's
  reviewed paths/hunks, leaving pre-existing changes and other tasks out of its commits. A task may have
  multiple verified incremental commits; record their IDs in that task's progress/journal.
- Before each commit inspect status, working/staged diffs and new files; run git diff --check and the
  relevant checks, including the privacy review. Stage explicit reviewed paths/hunks, never blanket-stage.
- Documentation-only increments use consistency/link/whitespace checks; pre-existing missing application
  infrastructure does not prevent such commits. Record broader failures without claiming feature success.
- For code increments pass applicable tests and retain evidence; broader unavailable/failing gates stay
  explicit. A commit is not a completion signal and must not bypass full feature verification.
- Use purpose-specific messages, update progress with commit IDs and report them to the user.
  Standing authorization covers local incremental commits unless the current request restricts them.
  Evidence-only follow-up commits are valid meaningful units.

## Commands and current infrastructure limits

```powershell
powershell -ExecutionPolicy Bypass -File scripts/setup-dev.ps1
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-bootstrap.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-unit.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-integration.ps1
powershell -ExecutionPolicy Bypass -File scripts/deploy-bve.ps1
powershell -ExecutionPolicy Bypass -File scripts/test-bve.ps1
powershell -ExecutionPolicy Bypass -File scripts/verify.ps1
```

Do not substitute ad-hoc build commands for repository scripts when reporting feature completion.
Deployment is only appropriate after the target, artifact, ownership and recovery checks above.
deploy-bve.ps1 currently copies one DLL; it does not implement those safeguards or certify host behavior.
test-bve.ps1 is still a deliberate failure placeholder.

The [configured-host bootstrap](specs/001-host-bootstrap/spec.md) is a bounded F001 increment.
setup-dev.ps1 generates ignored src/CommEx/CommEx.local.props from the configured Extensions parent;
the installed PluginHost reference has copy-local disabled and must not be redistributed by this setup.
test-bootstrap.ps1 checks the DLL entry contract offline; actual BveTs load requires separate host evidence.
deploy-bootstrap.ps1 and test-bve-bootstrap.ps1 provide the bounded configured-host deployment/load checks;
restore-bootstrap.ps1 restores a saved prior DLL. Follow the [build and recovery guide](docs/build-configuration.md).
Application unit/transport test assemblies and the full six-host harness remain pending. Bootstrap checks
do not replace scripts/verify.ps1 or complete F001; track their scope in the [bootstrap tasks](specs/001-host-bootstrap/tasks.md).

Do not launch autonomous-loop.ps1 for unattended work until its blanket staging, completion-only
commit prompt, per-invocation attempt reset and missing resume/approval/dependency controls are fixed.
The documented agent workflow is not a claim that the current runner implements it.
Track those prerequisites in the product task register.

## Definition of done

A feature is done only when all applicable acceptance criteria are satisfied, scripts/verify.ps1 exits 0,
no required stage was skipped, the tree contains no unintended changes and the report identifies the
tested commit and retained evidence. Durable state, documentation consistency and scoped commits are
part of completing each authorized work increment.
