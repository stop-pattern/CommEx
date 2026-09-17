# BveCommPlugin Constitution

## I. Specification is the contract

Externally observable behavior must come from an approved specification. Implementation may expose
missing or contradictory requirements, but must not invent protocol fields, failure behavior, or UI
semantics. Material ambiguity returns the feature to clarification or marks it BLOCKED.

## II. Verification is executable

Every feature has machine-checkable acceptance criteria. The canonical completion command is
`scripts/verify.ps1`. A written claim, successful compilation alone, mocked BVE behavior, or a
manually observed UI is not sufficient when a higher verification level applies.

## III. Four verification levels

1. Build: the solution builds with the configured Visual Studio MSBuild and no errors.
2. Unit: deterministic logic such as codecs, validation, state transitions, and configuration passes.
3. Integration: real sockets/serial abstractions and TestPeer interaction pass without BVE.
4. BVE E2E: the deployed DLL loads through BveEX in the fixed test scenario, its UI and communication
   path work, BVE remains responsive, and no unhandled exception is recorded.

Skipping level 4 is allowed only for components whose approved acceptance criteria explicitly state
that BVE E2E is not applicable. A missing tool or machine configuration is BLOCKED, not PASS.

## IV. Runtime isolation

BVE callback execution must remain bounded and non-blocking. Communication and durable writes run on
worker tasks/threads with bounded queues, cancellation, timeouts, and explicit overflow behavior.
UI creation is lazy. Closing the settings window must stop UI-only work and must not leak handlers.

## V. Failure is designed behavior

Disconnects, malformed input, unavailable ports, duplicate connections, shutdown during I/O, and
configuration corruption require specified, tested behavior. Exceptions do not escape into BVE.
Logs must be useful without containing secrets or unbounded high-frequency output.

## VI. Evidence and reproducibility

Every autonomous attempt records the command, exit code, relevant logs, tool versions, tested commit,
and timestamps. A passing feature receives a concise human-readable summary and machine-readable
`result.json`. Machine-local configuration and third-party binaries are never committed.

## VII. Bounded autonomy

Autonomous debugging has a finite attempt limit. Repeating the same hypothesis without new evidence is
forbidden. A blocked feature must state the failure, evidence, attempted hypotheses, and smallest human
decision or environmental action required. Independent work may continue.

## Governance

Changes to this constitution require an explicit human-approved commit. Feature plans must include a
constitution check. Reviews reject implementations that achieve visible behavior by violating thread,
lifetime, evidence, or verification requirements.

Version: 0.1.0  
Ratified: 2026-09-17

