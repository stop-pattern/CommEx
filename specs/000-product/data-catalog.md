# Public data and command catalog

Status: Draft inventory; product scope is confirmed, exhaustive API mapping is BLOCKED under B02/B07.
This is not the Native wire schema and does not certify API availability. Consult [specification](spec.md)
and [compatibility](compatibility.md) for authority and the six H01–H06 environments.

## Catalog contract

Cover all data-readable public host API values in the approved environments, including vehicle,
operating state and route data, not just values present in existing protocols. Numbers, strings,
enumerations and bounded collections can be represented as data. Raw windows/device objects,
unbounded traversal and arbitrary method execution are outside scope.

Each final catalog entry must identify:

- Stable Native field identifier, meaning, type, unit, range, precision and relevant missing-value state.
- Exact read API/member and availability in each of H01–H06, including required host extensions.
- Snapshot timing, lifetime, bounded collection/size behavior and any thread-affinity constraints.
- Mapping to every relevant legacy field/token, including differing units, sign conventions and door polarity.
- Read/write permission, index validation and independent acceptance fixture.
- For writes: permissible operations, value validation, host application point and conflict semantics.

The final catalog is an explicit deliverable of P002/P003. Rows below are initial coverage groups,
not a claim that all public APIs have already been enumerated.

## Initial read inventory

Identifiers in this table are documentation labels, not approved JSON field names. Every row still
needs H01–H06 mapping and bounded capture evidence.

| Data group | Candidate values / representation | Legacy mapping lead | Mapping status |
|---|---|---|---|
| Position | Train location, numerical distance | BIDS IE0; Communication.dll Location | Pending API/unit mapping |
| Speed | Signed speed; absolute speed where exposed | BIDS IE1; Communication.dll Speed/SpeedAbs | Pending sign/precision mapping |
| Simulation time | Elapsed milliseconds and clock components | BIDS IE2, IE10–13; Communication.dll Time | Pending time/pause semantics |
| Pneumatic pressures | BC, MR, ER, BP, SAP | BIDS IE3–7; Communication.dll corresponding fields | Pending unit/range mapping |
| Electrical state | Current and any other publicly readable values | BIDS IE8; voltage IE9 is reserved in the cited document | Pending; reserved fields are not evidence of available voltage |
| Handles | Power, brake, reverser, one-handle representation, constant-speed state where available | BIDS IH0–3; Communication.dll handle fields | Pending host distinction and reserved-field behavior |
| Vehicle specification | Power/brake notch counts, ATS check, B67, car count, emergency/max-service positions where exposed | BIDS IC0–4; Communication.dll specification fields | Pending exact meanings/ranges |
| Doors | Open/closed state and further public door data | BIDS ID0; Communication.dll IsDoorClosed | Pending conversion; do not assume identical polarity |
| Frame timing | Delta time and other public timing values | Communication.dll DeltaT | Pending type/unit and snapshot semantics |
| Panel | Values and supported indices | BIDS IPn; Communication.dll Panel | Pending actual host lengths and extensions |
| Sound | Sound state values and supported indices | BIDS ISn; Communication.dll Sound | Pending state/command interpretation |
| Further vehicle data | Data-readable public vehicle properties/collections | Native coverage beyond legacy formats | Exhaustive enumeration required |
| Route/scenario data | Data-readable route, station, signal, timetable, other-train and scenario values where publicly available | Native coverage beyond legacy formats | Exhaustive enumeration required; no assumed availability |
| Further public data | Remaining data-readable host API values | Native catalog and capabilities | Audit the API surface and document inclusion/unavailability |

BIDS identifiers are research mappings from the owner's memo and the
[public command document](https://gist.github.com/TetsuOtter/76b974d462276c9c993ddeb0b6a90cb0).
The document marks voltage and constant-speed entries as reserved; do not fabricate working values.
Existing formats may expose fewer indices/fields than Native. Preserve their contracts, and document
what cannot be represented rather than resizing packets or treating unavailable values as valid zeroes.

The [official host introduction](https://bveex.okaoka-depot.com/wiki/quickstart) describes a broad wrapper
API. Its breadth alone is not evidence that every getter is safe, bounded or available in every host.
BveEX's local PluginHost/CoreExtensions XML documentation provides additional mapping leads, but
member descriptions do not replace execution evidence on the older AtsEX and legacy paths.

## Write and input catalog

| Target | Product decision | Remaining contract work |
|---|---|---|
| Power/brake/reverser/one-handle | Accept approved commands from all active sources in common admission order | Exact host API, accepted ranges, notch conversion, invalid-value response and local input arbitration |
| Keys/buttons | Press/release with per-source ownership | Supported key list and event injection/overlap semantics; legacy key/button mappings |
| Panel indices | Configured allowlist; apply once; later vehicle updates may replace value | Index/type/range rules, allowed list configuration and precise host update point |
| Other writable public items | Research candidates only; individual approval required | Enumerate candidate APIs, conflicts, failure behavior and acceptance cases before enabling |
| Sound playback | Not approved merely because Sound is readable | Separate explicit write decision required |
| Raw object access / arbitrary methods | Outside scope | Must not become a generic remote invocation endpoint |

For the same command target, the later admitted command wins. Retain source ownership of key holds:
disconnecting one source does not release another source's hold. Preserve last handle positions on
disconnect, and apply only the approved one-shot Panel writes. Transport liveness detects the source
loss according to Native or legacy policy, not a universal inactivity rule.

On scenario change discard pending old commands, release keys and stop connections before choosing
the new profile. Source/session identification, replay rejection and queue overflow must be specified
under B03/B07 before this behavior can be implemented reliably.

## Native delivery coverage

- Request data or selectively subscribe from the external peer.
- Periodically send the configured selection of fields.
- Periodically send all catalog data available on the active host.
- Default periodic interval: 50 ms, configurable per connection. Finite message, snapshot and queue
  limits still apply; all-data mode does not waive the callback or memory bounds.
- Replace stale pending telemetry with current state when behind. Define splitting/backpressure and
  fairness in the Native/transport contract before claiming all-data works on slow Serial.
- Keep host-unavailable data distinguishable from valid values; finalize its JSON representation
  and the legacy formats' representable behavior through B01–B03.
