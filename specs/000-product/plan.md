# Product implementation plan

Status: Draft — aligned to the owner's 2026-09-17/18 product decisions.
This is the implementation roadmap, not a claim that features or contracts are complete.
[Specification](spec.md), [compatibility](compatibility.md), [data catalog](data-catalog.md) and
[acceptance](acceptance.md) govern the feature plans.

Execution follows [AGENTS.md](../../AGENTS.md) and the [development workflow](../../docs/development-workflow.md).
Maintain a goal, current progress and action journal under reports/<work-id>/ throughout each attempt,
including deployment state, evidence, consumed attempts and the next action. Delegate independent research
and separately owned implementation work; the coordinator verifies integration and owns shared Git/resources.

## Dependency order

1. Resolve the host bootstrap portion of B02/B08/B09 and specify F001. Establish the six-environment
   load/unload harness early, including a known-good plugin and a deliberately broken plugin.
2. Specify F002 configuration and F010 diagnostics after B05/B10. Define the profile/connection
   contracts for F016 early so persistence and UI do not invent incompatible structures.
3. Establish F012 host data/input mapping and F011 legacy reference contracts; resolve B01/B02/B07
   for each smallest independently testable increment.
4. Specify F013 Native JSON (B03) before its transport integrations. Approve bounds/defaults (B04)
   and UI/lifecycle transitions (B06), then implement F003 and the transport-independent F016 state machine.
5. Build legacy and Native codecs incrementally with independent vectors. Integrate F004/F005 UDP,
   F006/F007 TCP and F008 Serial, sharing codecs only for approved transport/format pairings.
6. Add F014 external-broker MQTT and F015 WebSocket client/server. Both are v1 requirements, not
   post-release options. Close each transport's F016 automatic-start/retry integration.
7. Complete F009 runtime application and profile/scenario transition integration across every transport.
8. Run R001 full regression across all six environments, then R002 packaging/license/evidence review.

Pure codec or state-machine increments may proceed independently after their own contracts are
Approved; full feature completion still requires their specified integration and host evidence.
Do not postpone the first BVE integration test until communication features are finished. Missing
BVE 5/AtsEX environments block their required stages and release, not unrelated contract research.

## GitHub CI/CD infrastructure

Implement [FR-020 and the CI/CD contract](ci-cd.md) as a separate infrastructure increment:
define executable code checks and runner/build setup; implement push checks/build/upload with one-day
artifact retention; reuse that pipeline for the tagged commit before creating a draft Release with
matching assets; verify CI-AC-01 through CI-AC-05. Use repository build scripts with runner-local
configuration, without committing personal settings. Resolve B09 for distributable artifacts and
define rerun behavior before updating existing drafts/assets.
Draft preparation may precede R001/R002; product publication still requires those gates.

## Architecture boundaries

| Boundary | Responsibility |
|---|---|
| Host adapters | Thin AtsEX and BveEX API bindings, lifecycle and bounded snapshot/input work; explicitly test BveEX legacy mode |
| Core | Host-independent typed data catalog, snapshots, ordered input admission and per-source key ownership |
| Codecs | Legacy serialization/parsing and Native JSON; no host, UI or socket dependencies |
| Transports | Serial, UDP, TCP client/server, MQTT client, WebSocket client/server; bounded I/O and cancellation |
| Connection coordinator | Multiple endpoints, auto-connect/reconnect state, finite budgets, peer/session lifetime and manual stop |
| Configuration/profiles | Environment-scoped versioned JSON, validation, backup, import/export and whole-profile selection |
| UI | Lazy WinForms controls, stable automation identifiers and explicit status/error display |
| Diagnostics | Bounded logs, privacy, rotation and bounded shutdown flush |
| TestPeer/harness | Independent reference vectors, real transports/broker/physical Serial loopback and host automation |

These are logical boundaries, not a requirement for one assembly per row. Prefer minimal runtime DLLs;
allow necessary host-specific builds and justified dependencies without mandatory assembly merging.
The user-authorized infrastructure scaffold is src/CommEx/CommEx.csproj, an SDK-style net48/AnyCPU library
built through CommEx.slnx (see [build settings](../../docs/build-configuration.md)). Host-specific release
artifacts and third-party package/reference decisions still await B09 and the corresponding feature technical plans.
Use the common README/AGENTS responsibility layout: production projects under src/, test levels under
tests/Unit, tests/Integration and tests/BveE2E, and the independent peer under tools/TestPeer.

## Interfaces and data flow

- Host callbacks read approved bounded snapshots and apply admitted commands at specified host update
  points. Network, Serial, process and file I/O run outside those callbacks.
- A codec maps a host-independent snapshot or command to its approved wire contract. Compatibility
  fields/units are preserved per format; Native uses the catalog and host availability metadata.
- Each connection selects a supported transport/format combination and owns its worker, queues and
  cancellation. Servers additionally own bounded per-client resources.
- State sending may replace stale queued state with the latest values. Input/response overflow rules
  remain B04/B07; do not apply state coalescing blindly to press/release events or acknowledgements.
- Input from all peers enters one defined admission order; later input wins for the same target.
  Source-owned keys permit correct cleanup even when several sources hold the same key.
- Native liveness operates on the command source/session. Legacy codecs retain their own handshake;
  optional inactivity timeout must not silently change held-key behavior.
- On scenario transition stop connections, discard old commands and release keys before selecting the
  vehicle, scenario or default profile. Start only the selected profile's auto-connect endpoints.
- Validate a proposed configuration as a whole before saving/applying. Invalid data leaves known-good
  data intact. A valid saved configuration is retained on endpoint connection failure; show failed
  endpoints as stopped and apply their finite retry policy. Atomic file replacement is not a promise
  that all external resources connect atomically. Resolve save/replacement failures through B05/B06.

## Constitution check

- Contract authority: decisions are recorded; unresolved B01–B10 are explicit implementation blockers.
- Verification: require the four applicable levels and six-environment evidence. A launch-only smoke
  test, virtual-only Serial or a skipped required stage cannot certify completion.
- Runtime isolation/lifetime: bounded callbacks, worker I/O, cancellation and deterministic cleanup.
- Failure design: independent reference vectors, malformed input, missing peers, finite retries,
  resource conflicts, corrupted settings and scenario-transition races all require acceptance tests.
- Evidence/autonomy: retain commands, exit codes, tools, timestamps, tested commit and hypotheses.
  Use the attempt limit in machine-local configuration; no unlimited autonomous retry.
- The constitution is unchanged. Apply its isolation and verification principles to both approved
  hosts; any actual governance change requires the separately specified human-approved commit.

## Quality gates and recovery

A feature may start only with an Approved specification, applicable blockers closed, its own
dependency-ordered tasks and acceptance criteria. Add tests before or alongside each coherent increment.
Run the narrowest relevant tests, then scripts/verify.ps1. On failure preserve evidence and test a
falsifiable hypothesis within the configured attempt budget. Persist the consumed budget across resumes;
record failed attempts and intermediate checkpoints, not only the final success report. Deployment requires
an identified artifact/target, exclusive resource ownership, recovery preparation and post-load observations.

Follow the [agent commit policy](../../AGENTS.md): commit each smallest meaningful increment after
its applicable checks pass, before moving to independent work. Keep related implementation/tests or
interdependent documents together; do not wait for the entire feature or session. Documentation-only
increments use content/link/whitespace checks. Record broader unavailable/failing gates explicitly;
an intermediate commit never marks a feature complete. Stage only reviewed, related changes.
One prompt can contain several such commits. Review tracked evidence and commit metadata for personal
or confidential data; keep raw machine-specific evidence in ignored locations.

Mark a feature complete only after applicable acceptance passes and the full verification command
succeeds without skipped required levels. Record reports using reports/result.schema.json, review the
change, and ensure no unrelated edits. A completed feature commit must be identifiable in the evidence.

Development rollback and runtime settings behavior are different: retaining valid new settings after
an unavailable peer is an approved runtime behavior, not grounds to roll back to the old configuration.
