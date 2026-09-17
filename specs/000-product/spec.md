# Product specification: CommEx

Status: Draft — product decisions were confirmed by the owner on 2026-09-17/18.
The decisions below are approved direction, not permission to invent unspecified contracts.
Features affected by an unresolved blocker remain BLOCKED for implementation. Each feature needs
its own Approved specification and acceptance criteria before implementation.

## Purpose

Provide an AtsEX/BveEX Extension connecting BVE to external applications and hardware through Serial,
UDP, TCP, MQTT and WebSocket. Support existing formats alongside Native v1 JSON, with a WinForms UI
and bounded, non-blocking work on BVE callback threads.

Normative supporting documents: [compatibility](compatibility.md), [data catalog](data-catalog.md),
and [acceptance criteria](acceptance.md). The [research memo](../../互換プロトコル.md) supplies research
leads; its examples, estimates and original recommendations do not override these decisions.

## Users and operating environment

- Primary user: repository owner operating BVE on Windows 11, with external displays/controllers.
- Runtime: .NET Framework 4.8. UI: WinForms. Development: native Windows PowerShell and VS/MSBuild.
- BVE baselines: 5.8.7554.391 and 6.0.7554.619.
- Host baselines: AtsEX 1.0-RC10 / 1.0.41005.1; BveEX 2.1.51225.1 normal and legacy modes.
- All six BVE/host combinations in the compatibility matrix require verification. Selecting them is
  not evidence that they work. Other versions are unverified until added through specification and tests.
- BVE 5 and other missing test paths will be supplied in machine-local configuration. Missing required
  environments block their verification and release, rather than becoming skipped passes.

## Clarifications

### Session 2026-09-17/18

Normalized records of the owner's answers, including later refinements:

- D01 Q: New protocol or existing compatibility? A: Both; existing compatibility is required in v1.
- D02 Q: Which existing formats? A: The five existing families in 互換プロトコル.md.
- D03 Q: Initial transports? A: Serial, UDP, TCP, MQTT and WebSocket, including the latter two in v1.
- D04 Q: Concurrent operation sources? A: Accept all connections; later accepted input wins for the same target.
- D05 Q: Disconnect behavior? A: Preserve handles; release the source's keys, retaining keys held by another source.
- D06 Q: Supported hosts? A: BVE 5 and 6, AtsEX and BveEX; supply BVE 5 paths later.
- D07 Q: Network scope? A: Same PC and trusted LAN; no Internet exposure or cloud MQTT in v1.
- D08 Q: Native read scope? A: All data-readable public host API values, subject to an explicit catalog.
- D09 Q: External writes? A: Handles/keys plus Panel and additional explicitly approved writable items.
- D10 Q: AtsEX meaning? A: Old AtsEX runtime, normal BveEX and BveEX legacy mode all require support.
- D11 Q: Settings scope? A: Environment-specific settings, common JSON import/export and vehicle/scenario profiles.
- D12 Q: Profile precedence? A: Vehicle, then scenario, then environment default; select one complete profile.
- D13 Q: Native encoding? A: JSON on every supported transport.
- D14 Q: Telemetry delivery? A: Request/selective subscription, configured periodic fields and all-data periodic
  delivery must be selectable, with bounded traffic and latest-state replacement when transmission falls behind.
- D15 Q: Panel policy? A: Apply once to permitted indices; allow later vehicle updates.
- D16 Q: Serial evidence? A: Physical-port loopback is mandatory; virtual ports alone cannot complete Serial.
- D17 Q: Responsiveness? A: Ten-minute run, average FPS loss at most 5%, 99% of accepted commands within 100 ms,
  shutdown within 5 s, under a fixed workload.
- D18 Q: Persistence/logging/release defaults? A: Adopt the defaults below. Minimize dependency DLLs; prefer
  one DLL but do not require merging dependency and project-generated DLLs.
- D19 Q: Concurrent endpoints? A: Multiple configurations and bounded multi-client TCP/WebSocket servers.
- D20 Q: All public information? A: Data values, not live window/device objects or arbitrary method execution.
- D21 Q: Silence versus disconnect? A: Native has liveness checks; legacy inactivity timeout is optional.
- D22 Q: Startup connection? A: Each transport/format connection gets an auto-connect checkbox.
- D23 Q: Retry behavior? A: Separate retry checkbox, finite retries, then stop and permit manual restart.
- D24 Q: Connection defaults? A: Adopt the connection table below.
- D25 Q: Scenario transition? A: Stop connections, clear pending input and keys, reselect the profile and auto-start.
- D26 Q: Exact versions? A: The six combinations of the fixed baseline versions above.
- D27 Q: MQTT/WebSocket roles? A: External LAN MQTT broker; WebSocket client and server; no embedded broker.
- D28 Q: Valid settings but connection failure? A: Save new settings and stop failed endpoints; retry only if enabled.

## Functional requirements

### Communication and data

- FR-001: Separate host adapters, transport I/O and codecs. The compatibility table defines supported
  pairings; separation does not mean every legacy codec works on every transport.
- FR-002: Include Communication.dll, BIDS ASCII v202, BIDS Binary framing, BveSerialOutput and NCI
  compatibility in v1. Preserve verified wire behavior; do not add Native fields to legacy frames.
- FR-003: Native v1 uses JSON on Serial, UDP, TCP, MQTT and WebSocket for retrieval and approved commands.
  Framing, field identifiers and version handling require the Native contract.
- FR-004: Catalog all data-readable public values across supported hosts, including operating state,
  vehicle, route, Panel and Sound. Record types, units, host availability and conversion rules. Unavailable
  data must be identifiable as unavailable; the wire representation is still to be specified. Do not
  expose raw UI/device objects or arbitrary method invocation.
- FR-005: Native offers request/selective-subscription, configured-field periodic and all-data periodic
  delivery. Default periodic interval: 50 ms, configurable per connection. All-data means the approved
  catalog available on the active host, not recursive host object serialization. Bound capture, encoding
  and transmission; replace stale queued state with latest state when behind.
- FR-006: Support multiple endpoint configurations, including several of the same transport. TCP and
  WebSocket servers accept multiple clients with specified finite limits. MQTT connects to an external
  trusted-LAN broker. WebSocket supports client and server roles.
- FR-007: Accept approved handle/key commands from all connections through a common ordered admission
  path. Later accepted commands win for the same target. Track key ownership per source so disconnect
  cleanup cannot release a key another source holds. Local input arbitration remains B07.
- FR-008: Allow Panel writes only to configured permitted indices. Apply each accepted write once at the
  specified host update point; subsequent vehicle updates may replace it. Additional writes need an
  explicit approved catalog entry; Sound playback is not implicitly authorized.

### Connections and lifetime

- FR-009: Each connection has separate auto-connect and reconnect checkboxes. Auto-connect means opening
  the Serial port, starting UDP I/O, connecting a client or starting a server listener, according to role.
  Start profile-specific connections after the vehicle/scenario is known and the profile is selected.
- FR-010: Use these accepted defaults. Legacy-specific negotiation takes precedence; exceptions and
  configurable ranges must be explicit in the feature contract.

| Setting | Accepted default / rule |
|---|---|
| New connection auto-connect | OFF |
| New connection reconnect | OFF |
| Retries after initial failure | At most 5; delays of 1, 2, 4, 8, 16 seconds after preceding failure |
| Connection-attempt timeout | 5 seconds per attempt |
| Retry budget reset | Manual connect, or a connection stable for at least 30 seconds |
| Retry exhaustion | Stop that connection, display cause; manual connect can restart it |
| Manual stop | Cancel in-flight operations and retries; no automatic restart in that activation |
| Native liveness | 1-second checks; 5 seconds without response is a disconnect |
| Legacy inactivity timeout | Optional; disabled unless enabled |

- FR-011: Recognized disconnect, I/O failure or manual stop releases that source's keys and preserves
  handle positions. Native liveness detects silent peers; legacy silence alone is not disconnect unless
  its optional timeout is enabled. Specify peer identity and liveness per transport, especially UDP/MQTT;
  broker health must not stand in for command publisher health.
- FR-012: On vehicle/scenario transition stop old connections, discard pending commands and release keys.
  Select the new complete profile, then start only auto-connect-enabled endpoints. Do not replay old
  commands into the new scenario.

### Settings, UI and persistence

- FR-013: Keep settings independent per environment, using a common versioned JSON import/export format.
  Select vehicle profile first, otherwise scenario profile, otherwise environment default. Select one
  whole profile without overlaying or merging.
- FR-014: Store settings and logs under `%LocalAppData%\CommEx\<environment-id>`. Define environment identity
  and matching under B05. Machine-specific paths belong in ignored local configuration. Declare external
  write/deployment paths there before tooling writes or installs outside the repository.
- FR-015: Validate the whole configuration first. Invalid, partially parsed or unknown schema versions
  cannot overwrite known-good settings. Save by atomic replacement and retain the preceding backup.
  Legacy CommEx/other product configuration migration is outside v1; new-format import/export is included.
- FR-016: Valid new settings remain saved when a port/peer is unavailable. Failed endpoints stop and expose
  the cause, with bounded reconnect if enabled. Connectivity failure is not validation failure. Specify
  save failures and resource replacement before F009 implementation.
- FR-017: Create the WinForms UI lazily with stable automation identifiers, profile/endpoint editing,
  auto-connect/reconnect checkboxes, status and manual connect/stop. Closing/reopening must not leak
  UI resources. Detailed controls and validation ranges remain B06.

### Diagnostics and distribution

- FR-018: Default logs include information, warnings and errors, bounded to five files of 10 MiB each.
  Full communication payload logging is OFF by default. Avoid secrets, bound high-frequency output,
  and specify shutdown flushing and diagnostic opt-in behavior before implementation.
- FR-019: Minimize runtime dependencies, ideally distributing one plugin DLL. Do not require merging
  dependency or project-generated DLLs solely for that ideal. Allow necessary host-specific builds and
  justified dependencies with notices. Include configuration examples and documentation; do not
  redistribute BVE/AtsEX/BveEX runtimes. Exact artifacts/dependencies need license review.

## Cross-cutting requirements

- No network, Serial, process or file I/O waiting on BVE/AtsEX/BveEX callbacks.
- Queues, clients, messages, snapshots and parser allocations have finite, specified limits. Latest-state
  replacement does not authorize silently dropping input events or responses; their overflow rules need approval.
- I/O supports timeout/cancellation. Detach events, stop workers and release ports, sockets and UI on disposal.
- Malformed input and unavailable resources cannot escape as exceptions into BVE.
- Support disabling/disconnecting without restarting BVE where the approved host API permits it.
- Under a frozen workload: ten-minute run, average FPS loss at most 5% versus baseline, at least 99% of
  accepted commands applied within 100 ms, shutdown within 5 seconds.
- Physical Serial loopback and all six host combinations require evidence. See [acceptance](acceptance.md).

## Initial feature map

Each capability receives its own numbered Spec Kit feature directory before implementation. Existing IDs
are retained. Recording a decision does not complete an implementation task.

| ID | Capability | Required objective evidence |
|---|---|---|
| F001 | Host adapters, bootstrap, lifecycle and E2E harness | Six-host load/unload matrix; known-good/broken controls |
| F002 | Versioned configuration, import/export, backup | Round-trip, corrupt/invalid/unknown input, atomic replacement |
| F003 | Lazy WinForms UI | Automation, checkboxes, reopen and error states |
| F004 | UDP transmit | Independent peer validates format, addressing and bounded cadence |
| F005 | UDP receive | Ordered validated input; malformed/lost/duplicate input behavior |
| F006 | TCP client | Connect, finite retries, framing, timeout and shutdown |
| F007 | TCP server | Bounded multiple clients, isolation and shutdown |
| F008 | Serial I/O | Physical loopback, protocol fixtures, I/O failure and device removal |
| F009 | Runtime configuration apply | Valid settings retained on connection failure; invalid settings rejected |
| F010 | Logging and diagnostics | Rotation, privacy, bounded sink and shutdown flush |
| F011 | Five legacy codec families | Pinned reference contracts and independent byte/message vectors |
| F012 | Public data catalog and external commands | Host coverage, ordering, key ownership, allowed Panel writes |
| F013 | Native v1 JSON | Approved contract and delivery modes over all five transports |
| F014 | MQTT client | Real external broker, publisher lifecycle, approved message policy |
| F015 | WebSocket client/server | Both roles, multiple clients, Native messages and lifecycle |
| F016 | Profiles and automatic connection lifecycle | Precedence, scenario reset, auto-start and finite retries |

## Out of scope for the first release

- Internet exposure, cloud MQTT, remote authentication service and an embedded MQTT broker.
- HTTP/REST API, WebRTC/SkyWay, automatic updater and installer packaging.
- Frameworks other than the specified AtsEX/BveEX baselines; guarantees for untested versions.
- Raw host object export, arbitrary method execution and writes absent from the approved catalog.
- Old product configuration migration and mandatory DLL merging.

## Implementation blockers

The original eleven open decisions map to the requirements above and the remaining work below.
Every row is BLOCKED until its stated evidence/decision exists. This documentation update can finish
with these visible; affected feature implementation and release cannot.

| ID | Remaining decision or evidence | Affected features | Resolution required |
|---|---|---|---|
| B01 | Exact legacy contracts and reference revisions | F011, F004–F008 | Verify packing, offsets, byte order, framing, commands/responses, version/error rules, template grammar and fixtures; resolve discrepancies explicitly |
| B02 | Exhaustive API catalog across six hosts | F001, F012, F013 | Record every value's type/unit/API/availability and bounded capture; enumerate additional write candidates separately |
| B03 | Native wire contract | F013–F015, F004–F008 | Approve JSON schema, encoding/framing, versions/errors, request/subscription IDs, sessions/liveness, UDP loss/reordering, MQTT topics/QoS/retention, WebSocket options |
| B04 | Transport limits and defaults | F004–F008, F014–F016 | Fix addressing/ports, packet/message/client/queue limits, overflow, Serial baud/parity/data/stop bits/flow control, read/write timeouts and hot-plug identity |
| B05 | Persistence/profile contract | F002, F009, F016 | Fix environment IDs, schema, matching/duplicates, path portability, corruption recovery and save failure behavior |
| B06 | UI and configuration transitions | F003, F009, F016 | Define ranges, startup before scenario selection, profile editing, concurrent edits, resource replacement and status transitions |
| B07 | Host input/write semantics | F012, F005, F008, F013 | Confirm local input arbitration, key/handle ranges, Panel update point, stale/replayed input rejection and command overflow; approve additional writes |
| B08 | Test environment and workload | F001 and release | Supply missing host paths, physical fixture, scenario permission/selectors; freeze connections, rates, datasets and measurement procedure |
| B09 | Artifacts/dependency license evidence | F001, F014, F015, R002 | Select minimal compatible dependencies, host-specific layout and notices; confirm reference assembly usage/distribution terms |
| B10 | Diagnostic details | F010 | Specify log names, opt-in payload privacy/retention, sink overflow and bounded flush within shutdown budget |

See [plan](plan.md) for dependencies and [tasks](tasks.md) for the unchecked work register.

## Product completion

Release requires all required feature specs Approved, all applicable acceptance criteria passed and
scripts/verify.ps1 exiting 0 from a clean checkout on the reference environments, with no required
stage skipped. Reports identify tested commit, exact host/peer versions, limitations and retained
evidence. This document alone does not certify compatibility or verification success.
