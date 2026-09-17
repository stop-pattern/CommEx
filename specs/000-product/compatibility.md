# Compatibility targets and evidence

Status: Required v1 targets confirmed on 2026-09-18; compatibility has NOT been demonstrated.
[Product specification](spec.md) is normative. This matrix is not a list of shipped capabilities.

## Host matrix

| ID | BVE | Runtime / mode | Required release evidence | Current evidence |
|---|---|---|---|---|
| H01 | 5.8.7554.391 | AtsEX 1.0.41005.1 (1.0-RC10) | Load, UI, transport/input, performance, disposal | Environment and E2E pending |
| H02 | 5.8.7554.391 | BveEX 2.1.51225.1 normal | Same | Environment and E2E pending |
| H03 | 5.8.7554.391 | BveEX 2.1.51225.1 legacy | Same; AtsEX-facing plugin path | Environment and E2E pending |
| H04 | 6.0.7554.619 | AtsEX 1.0.41005.1 (1.0-RC10) | Load, UI, transport/input, performance, disposal | Environment and E2E pending |
| H05 | 6.0.7554.619 | BveEX 2.1.51225.1 normal | Same | Executable/host file versions inspected; no E2E proof |
| H06 | 6.0.7554.619 | BveEX 2.1.51225.1 legacy | Same; AtsEX-facing plugin path | Legacy-mode environment and E2E pending |

The owner will add missing BVE 5 paths as needed. Keep absolute paths in ignored local configuration.
A required environment that is absent remains BLOCKED; running H05 alone cannot certify the matrix.

BveEX changed AtsEX namespaces/APIs and provides a separate legacy mode; normal and legacy plugins
cannot simply be assumed to coexist in the same host session. Verify these as separate test runs and
select host adapters/artifacts accordingly. See the [official 2.0 changes](https://github.com/automatic9045/BveEX/discussions/44).
BVE 5 preparation must follow the [official BVE 5 instructions](https://bveex.okaoka-depot.com/download/bve5support).
The chosen baseline releases are [AtsEX RC10](https://github.com/automatic9045/BveEX/releases/tag/v1.0.41005.1)
and [BveEX 2.1](https://github.com/automatic9045/BveEX/releases/tag/v2.1.51225.1).

## Required transport / format pairings

Directions are relative to the plugin. Negotiation/control traffic may be bidirectional even for an
output or input format; the reference contract must define that traffic.

| Format | Required transport / role | Application direction | Contract status |
|---|---|---|---|
| Communication.dll compatible | UDP | BVE to external | B01: exact binary layout and packet behavior unverified |
| BIDS ASCII v202 | Serial; UDP; TCP client/server | Bidirectional | B01: complete command, reply and session behavior pending |
| BIDS Binary framing | Serial baseline | Bidirectional where defined by reference | B01: underlying payload and supported operations pending |
| BveSerialOutput compatible | Serial | BVE to external | B01: configurable text/template behavior pending |
| NumerousControllerInterface (NCI) | Serial | External controller to BVE | B01/B07: initialization, commands and mapping pending |
| Native v1 JSON | Serial; UDP; TCP client/server | Bidirectional | B03: full wire contract pending |
| Native v1 JSON | MQTT client to external LAN broker | Bidirectional | B03: MQTT/topic/session contract pending |
| Native v1 JSON | WebSocket client/server | Bidirectional | B03: framing/options/session contract pending |

Every pairing above is a v1 obligation. BIDS Binary over other transports requires reference research;
it is neither silently excluded from that research nor certified by the Serial framing description.
TCP/WebSocket servers require bounded multiple clients; all transports support multiple configurations.
Do not automatically expose every legacy codec on MQTT/WebSocket or confuse transport independence
with verified interoperability. Native is JSON even on UDP/TCP; the memo's Native binary proposal was superseded.

## Reference reconciliation checklist

| Family | Research lead | Evidence needed before contract approval |
|---|---|---|
| Communication.dll | Memo reports UDP 9032, header 0xFEFEF0F0, Panel/Sound arrays and a calculated 2152-byte packet | Pin reference source/version; determine offsets, padding, byte order and actual packet size; retain independent packet vectors and field-level assertions |
| BIDS ASCII | v202 command document; TR prefix and operation/data identifiers | Pin document/source revisions; determine reply formatting, bulk requests, errors, subscriptions, line endings, version negotiation and any TCP port redirection |
| BIDS Binary | Memo describes B64E plus Base64 and newline on Serial | Distinguish the outer framing from binary payload structure, byte order, update types, errors and supported transports |
| BveSerialOutput | Memo describes customizable text/CSV output | Identify authoritative reference/version; document template tokens, formatting/culture, precision, encoding, terminators, escaping and cadence; fixed CSV alone is insufficient |
| NCI | Published controller protocol repository | Pin version; verify exact tokens including original spelling, initialization/acknowledgements, port settings, button semantics, notch conversion and bounds |

The 2152-byte Communication.dll size is a research estimate, not an approved constant. Likewise
9032/14147 ports and reported Serial defaults are leads to confirm under B01/B04, not global Native defaults.
Do not change an existing wire protocol to enforce a new heartbeat or common newline convention.

The [BIDS v202 document](https://gist.github.com/TetsuOtter/76b974d462276c9c993ddeb0b6a90cb0)
omits details for bulk requests and errors; reading its command table alone is insufficient.
Other primary leads: [BIDS project](https://tralsys.github.io/BIDS/),
[BIDS server](https://github.com/Tralsys/BIDS_Server),
[BIDS Serial module](https://github.com/Tralsys/BIDSid_SerCon),
[NCI](https://github.com/kusaanko/BveNumerousControllerInterface).
The owner's [research memo](../../互換プロトコル.md) retains the original source list and examples.
No latest upstream revision is implicitly the compatibility target: record commit/tag and fixture origin.

## Native contract and deployment constraints

Native shares field meaning across transports, exposes host capabilities/unavailability and supports
the approved read/write catalog. It must specify JSON schema/encoding, stream framing, packet limits,
version/error behavior, message correlation, subscription/all-data modes, peer identity and liveness.

Transport-specific details remain B03/B04: UDP boundaries/loss/reordering and oversized snapshots;
Serial stream boundaries and speed limits; TCP framing and client bounds; MQTT protocol version,
topics, QoS, retained/replayed commands and publisher liveness; WebSocket roles/message limits and
connection options. Do not invent defaults in implementation while these are BLOCKED.

Network scope is same PC/trusted LAN. MQTT uses an external broker and WebSocket has both roles.
Cloud MQTT, Internet exposure, an embedded broker, HTTP/REST and WebRTC/SkyWay are outside v1.

## Distribution and licensing

Prefer minimal DLL count and one plugin DLL where feasible. Necessary host-specific outputs and
dependencies are allowed; merging dependency/project DLLs is not mandatory. Package examples,
documentation and required notices. Do not bundle BVE/AtsEX/BveEX runtimes or commit third-party binaries.

Before release, record the exact dependency versions, license sources, redistribution obligations,
host-provided assemblies and runtime deployment manifest (B09). The repository license does not
establish the licensing terms of every reference, dependency or scenario.
