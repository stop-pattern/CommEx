# Product implementation plan

Status: Draft

## Dependency order

1. F001 bootstrap/lifetime and the E2E smoke-test path.
2. F002 configuration and F010 diagnostics.
3. F003 settings UI.
4. F004/F005 UDP.
5. F006/F007 TCP.
6. F008 serial.
7. F009 runtime reconfiguration.
8. Full regression, packaging, and release report.

F001 deliberately includes the E2E harness. Deferring BVE integration until the end creates the largest
risk: a design can pass unit tests while being unloadable, blocking BVE, or incompatible with BveEX.

## Architecture boundaries

- `Plugin`: thin BveEX adapter and lifecycle ownership.
- `Core`: protocol-independent models, snapshots, state and orchestration.
- `Communication`: transports and bounded queues behind interfaces.
- `Configuration`: schema, validation, migration, atomic persistence.
- `UI`: lazy WinForms views and presenters/view-models; no transport logic.
- `Diagnostics`: structured events and bounded log sink.
- `TestPeer`: deterministic external UDP/TCP/serial peer for integration/E2E tests.

Concrete project names and package versions are selected in the F001 technical plan after supported
BveEX versions and redistribution constraints are confirmed.

## Quality gates per feature

- Specification Approved and ambiguity check complete.
- Constitution check complete.
- Tests identify success, relevant boundary conditions, and failure behavior.
- `scripts/verify.ps1` succeeds with all applicable levels enabled.
- Evidence is recorded using `reports/result.schema.json`.
- Code and test review find no unresolved Critical/High issue.

## Rollback and recovery

Each feature uses a separate commit after passing verification. Generated evidence identifies that
commit. A failed attempt is not committed as completed work. The autonomous runner may retain local
diagnostic changes, but must describe them when blocking or before moving to unrelated work.

