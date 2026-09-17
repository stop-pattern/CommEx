# Product acceptance and verification matrix

Status: Product-level criteria confirmed; detailed fixtures/workload remain BLOCKED under B08.
These are requirements, not test results. Feature specs must refine them into executable assertions
without weakening the fixed thresholds. See [specification](spec.md) and [compatibility](compatibility.md).

## Required scenarios

Levels: U = unit, I = integration with real I/O, E = BVE E2E. Build is required for every implemented feature.
A feature's approved spec determines applicability; missing tools/hardware never make a stage inapplicable.

| ID | Scenario and observable pass condition | Required evidence / levels |
|---|---|---|
| AC-01 | Each of H01–H06 loads the correct plugin, exposes the settings entry and unloads without unhandled exceptions | Exact host/mode/assembly versions, load/disposal events and E2E artifacts; E |
| AC-02 | Approved data catalog values agree with independent host observations; unavailable entries are reported as such; Native request, subscription, selected-field and all-data modes work | Per-host catalog coverage, captured expected/actual values, bounded slow-consumer tests; U/I/E |
| AC-03 | Each required format/transport pairing matches independently sourced bytes/messages; malformed/truncated/oversized messages and incompatible versions follow the approved contract | Pinned reference revisions, golden vectors and real peer captures; self-round-trip alone is insufficient; U/I/E |
| AC-04 | Two or more sources affect the same handle in admission order; key holds track each source; permitted Panel writes apply once and denied indices do not change | Recorded admission/application order, overlap/release assertions, controlled subsequent vehicle update; U/I/E |
| AC-05 | Disconnect leaves handles unchanged and releases only lost-source keys; Native silent-peer timeout works; legacy idle peers remain active unless timeout enabled | Fake-time state tests plus transport/host observations, including publisher loss while MQTT broker stays alive; U/I/E |
| AC-06 | Environment isolation, JSON import/export, atomic save and backup work; invalid/corrupt/unknown input preserves known-good settings; valid settings survive unavailable peer/port | File contents and runtime assertions, simulated persistence/resource failure; U/I/E for runtime application |
| AC-07 | Vehicle profile wins over scenario, scenario over default, with no merging; scenario change stops old connections, clears pending input/keys and starts only selected auto-connect endpoints | Competing profiles, transition during queued input/retry, no old command reaching new vehicle; U/I/E |
| AC-08 | Auto-connect and reconnect flags operate independently; finite retry schedule and stop/reset behavior match FR-010 | Clock-controlled tests, real failure/recovery, exhaustion status and manual restart; U/I/E |
| AC-09 | Multiple endpoint configurations and bounded server clients operate concurrently; stalled/malformed peers do not monopolize work; I/O resources close | Real UDP/TCP/Serial/MQTT/WebSocket peers, listener/client lifecycle, resource evidence; I/E |
| AC-10 | UI supports profile/endpoint edits, both checkboxes, connect/stop, state/error display, apply and close/reopen with stable automation IDs | Automated inspect/invoke/set-value and reopen evidence; E |
| AC-11 | Default info/warning/error logs rotate within five 10-MiB files, payload text is off, secrets are absent, floods and flush remain bounded | Rotation/privacy/overflow and shutdown tests; U/I/E |
| AC-12 | Fixed ten-minute workload meets FPS and command-latency limits below with required connections active | Paired baseline/enabled runs, workload manifest and raw measurements; E |
| AC-13 | Stop/unload/exit completes resource cleanup within 5 seconds even during I/O, retries and UI activity | Timed cleanup, detached events, no surviving workers/owned handles/ports; I/E |
| AC-14 | Release contains only approved host-specific plugin artifacts, justified dependencies, examples/docs/notices; clean checkout full verification succeeds | Artifact manifest/license review, DLL-count rationale and all required stage results tied to tested commit |

For AC-08, test new endpoints with both flags OFF; auto-connect with reconnect OFF; manual initial
connection with reconnect ON; and both flags ON. After initial failure there are at most five retries,
delayed 1/2/4/8/16 seconds after preceding failure; attempts time out after 5 seconds. Short successful
connections must not reset the budget before 30 seconds. Manual connect resets it; manual stop cancels
pending attempts/retries. Later scenario activation follows the new selected profile.

For AC-09, cover TCP and WebSocket in both client/server roles, MQTT with a real external LAN broker,
and Serial using actual physical-port loopback. Virtual COM tests may supplement but never replace the
physical test. Test reference protocol behavior separately from loopback: loopback alone proves neither
BIDS nor NCI compatibility. Physical branded controller models were not selected as mandatory fixtures.

## E2E scenario and environment preparation

Use the configured Uchibo 3793F ATS-P / E217 scenario as the reference candidate already identified
in local configuration. Its file exists; its suitability, local-use permission and automation selectors
still require P009 evidence on all six environments. Do not redistribute scenario assets or alter
external files outside declared local paths.

Each environment run must cover readiness and scenario load, host/plugin load, settings UI actions,
actual TestPeer communication and approved state changes, configuration/profile transition, failures,
responsiveness and cleanup. Log/screenshot evidence accompanies machine assertions; screenshots alone
do not pass the test. A deliberately broken plugin must fail and a known-good smoke plugin must pass
before trusting the harness.

The existing scripts/configuration describe one environment and the E2E script is still a deliberate
failure placeholder. Extend them during the infrastructure feature; this document does not imply that
the six-environment runner, selectors or application tests already exist.

## Performance acceptance

The numerical limits are confirmed. Freeze the workload before acceptance testing, including:

- Host/mode/bitness, machine/tool versions, scenario/vehicle/assets and plugin/baseline versions.
- Repeatable scenario actions, simulation settings, FPS measurement, warm-up and ten-minute measurement window.
- Number and roles of endpoints/clients, codec pairings, telemetry fields/modes/rates, Serial line settings,
  broker configuration, command load and injected failure conditions.
- Measurement clock, admission and host-application timestamps, handling of paused/unloaded states,
  collection overhead and the independent evidence source.

Pass thresholds:

| Measure | Required limit |
|---|---|
| Average FPS loss | At most 5%: 100 × (baseline FPS − enabled FPS) / baseline FPS, positive baseline, comparable runs |
| Command application latency | At least 99% of admitted commands applied within 100 ms, measured admission to host application |
| Observation duration | Ten minutes for the fixed performance run |
| Shutdown | At most 5 seconds from stop/unload/exit initiation to required resource cleanup |

Record rejected/lost/unapplied commands separately; never discard them silently to improve latency.
These limits do not assert Internet end-to-end latency or unlimited performance under arbitrary load.
Queue/message/client limits and overload behavior still require approved contracts (B03/B04/B07).
Exact workload values and detailed measurement rules are not yet approved, so performance acceptance
remains BLOCKED, even though its thresholds are fixed.

## Evidence and completion

Run repository scripts in order: build.ps1, test-unit.ps1, test-integration.ps1, test-bve.ps1 through
scripts/verify.ps1. Keep each environment's results in retained evidence; the aggregate cannot pass
unless every required environment/stage passes. A missing environment, missing fixture, placeholder
harness or skipped required stage is BLOCKED, not PASS.

Reports must include command/exit code, timestamps, tested commit, tool/host/peer versions, scenario and
workload identity, limitations and evidence paths. Feature result.json must conform to
[the result schema](../../reports/result.schema.json). Do not mark feature tasks complete based on
document checks. Documentation-only verification reports must be clearly separate from feature results.
