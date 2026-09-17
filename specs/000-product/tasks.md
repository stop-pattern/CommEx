# Product task register

Status: Product decisions recorded on 2026-09-18; implementation and verification remain pending.
Only check a feature after its own Approved spec exists and all required acceptance evidence passes.
[Blockers](spec.md), [compatibility](compatibility.md), [data catalog](data-catalog.md) and
[acceptance](acceptance.md) provide the source of truth.

## Contract and evidence prerequisites

These tasks resolve B01–B10; completing documentation is not evidence that a feature works.

- [ ] P001 Pin reference revisions for all five legacy codec families; produce exact wire contracts and independent golden vectors (B01, F011).
- [ ] P002 Enumerate data-readable public APIs across the six host combinations, recording types, units, limits, conversions and availability (B02, F001/F012).
- [ ] P003 Approve command ranges, local input arbitration, source identity/key ownership, Panel update point and any additional writable items (B07, F012).
- [ ] P004 Approve Native JSON schema, framing/version/error rules, request/subscription modes, session liveness and UDP delivery policy (B03, F013).
- [ ] P005 Approve MQTT version/topics/QoS/retention, publisher lifecycle and WebSocket role/options contracts (B03, F014/F015).
- [ ] P006 Specify transport addressing, packet/message/client/queue bounds, overflow, Serial defaults/read-write timeouts/hot-plug behavior and retry exceptions (B04).
- [ ] P007 Define environment IDs, JSON schema, profile matching/duplicates, import portability, corruption recovery and save failures (B05).
- [ ] P008 Specify UI ranges, startup/profile transition states and runtime resource replacement behavior, including valid settings retained on connect failure (B06).
- [ ] P009 Prepare missing local environments, physical loopback fixture, scenario permission/selectors and the fixed performance workload (B08).
- [ ] P010 Select minimal dependencies, host-specific artifact layout and verify licensing/notice obligations (B09).
- [ ] P011 Specify bounded diagnostic sink, file naming, payload opt-in privacy/retention and shutdown flush (B10).
- [ ] P012 Create and approve numbered feature specs/plans/tasks with acceptance IDs before starting each affected feature.

## Feature work in dependency order

- [ ] F001 AtsEX/BveEX adapters, bootstrap/lifetime/build path and six-environment BVE E2E harness (AC-01, AC-12, AC-13).
- [ ] F002 Versioned environment configuration, validation, atomic save/backup and common JSON import/export (AC-06).
- [ ] F010 Bounded logging, diagnostics, privacy, retention and flush (AC-11).
- [ ] F012 Host-independent public data catalog, ordered commands, per-source keys and permitted one-shot Panel writes (AC-02, AC-04, AC-05).
- [ ] F011 Communication.dll, BIDS ASCII v202, BIDS Binary, BveSerialOutput and NCI codecs with independent fixtures (AC-03).
- [ ] F013 Native v1 JSON contracts, retrieval/subscriptions and selected/all-data periodic modes (AC-02, AC-03).
- [ ] F003 Lazy WinForms UI, stable automation identifiers, profiles/endpoints/status, auto-connect/reconnect checkboxes and manual actions (AC-10).
- [ ] F016 Whole-profile selection, connection lifecycle, finite retries and scenario resets; integrate every transport as it becomes available (AC-07, AC-08).
- [ ] F004 UDP transmission for approved pairings and bounded delivery (AC-03, AC-09).
- [ ] F005 UDP reception, source/session handling and malformed/lost/replayed input behavior (AC-04, AC-05, AC-09).
- [ ] F006 TCP client, approved framing, connection timeout and finite retries (AC-03, AC-08, AC-09).
- [ ] F007 TCP server with bounded multiple clients and isolated cleanup (AC-04, AC-09).
- [ ] F008 Serial I/O, physical-port loopback, hot-plug and legacy/Native integration (AC-03, AC-08, AC-09).
- [ ] F014 External trusted-LAN MQTT broker client with Native data/commands and publisher liveness (AC-03, AC-05, AC-09).
- [ ] F015 WebSocket client and bounded multi-client server with Native data/commands (AC-03, AC-08, AC-09).
- [ ] F009 Runtime settings/resource replacement: reject invalid input, retain valid settings on endpoint failure and apply profile transitions (AC-06, AC-07, AC-08).
- [ ] R001 Clean-checkout full regression and fixed-workload performance checks on all six host combinations (AC-01–AC-13).
- [ ] R002 Minimal release artifacts, notices and final compatibility/limitations/evidence report (AC-14).

F016 has a transport-independent increment and later per-transport integration increments; do not check
it complete after the state machine alone. F001 establishes the early smoke harness; later feature
evidence extends it to real codecs/transports without weakening the known-good/broken controls.

## Infrastructure prerequisites

The six supported combinations are decided in spec.md. Environment preparation and verification are
separate, still-uncompleted tasks.

- [ ] Record all six host versions, modes, architectures and isolated test paths in local configuration.
- [ ] Extend the single-environment verification/deployment configuration to run and retain the full matrix.
- [ ] Declare every external fixture, settings/log and deployment write path before tooling uses it.
- [ ] Prove the build script selects the intended MSBuild and .NET Framework 4.8 target.
- [ ] Select unit/integration runners and deterministic time control for retry/liveness tests.
- [ ] Confirm the configured Uchibo 3793F ATS-P scenario's local-use permission and suitability on all hosts.
- [ ] Select UI Automation backend and freeze selectors (winapp preferred; isolated FlaUI fallback).
- [ ] Prove a deliberately broken plugin fails E2E and a known-good smoke plugin passes.
- [ ] Provide a real external MQTT broker, independent peers/vectors and physical Serial loopback fixture.
- [ ] Freeze workload and measurement rules before performance acceptance; do not tune them after failure.

No checkbox was completed by the clarification/documentation update.
