# Product specification: BVE communication plugin

Status: Draft — implementation must not begin until the open decisions are resolved and status is Approved.

## Purpose

Provide a BveEX Extension that obtains approved BVE state, exchanges data with external applications or
hardware over UDP, TCP, and serial communication, and offers a WinForms settings UI without degrading
BVE responsiveness.

## Users and operating environment

- Primary user: repository owner operating BVE on Windows 11.
- Host: BVE Trainsim with a compatible BveEX release.
- Runtime target: .NET Framework 4.8.
- Development: Visual Studio / VS Code and Codex CLI on native Windows PowerShell.

## Initial feature map

| ID | Capability | Required objective evidence |
|---|---|---|
| F001 | Extension bootstrap and clean shutdown | BveEX load evidence; no unhandled exception; disposal log |
| F002 | Versioned configuration load/save | round-trip, invalid/corrupt input, atomic replacement tests |
| F003 | Lazy WinForms settings UI | UI Automation inspect/invoke/set-value; reopen test |
| F004 | UDP transmit | TestPeer receives and validates a packet while BVE stays responsive |
| F005 | UDP receive | TestPeer input changes only the approved plugin state |
| F006 | TCP client | connect/reconnect/timeout/shutdown tests |
| F007 | TCP server | single/multiple-client policy and shutdown tests |
| F008 | Serial I/O | loopback or declared hardware fixture test |
| F009 | Runtime configuration apply | valid atomic change; invalid change rejected; no resource leak |
| F010 | Logging and diagnostics | bounded output, useful errors, no secrets, shutdown flush policy |

Each capability receives its own numbered Spec Kit feature directory before implementation.

## Cross-cutting requirements

- No blocking communication or file operation on a BVE/BveEX callback thread.
- Queues are bounded and have a specified overflow policy.
- Network and serial operations have timeouts and support cancellation.
- Malformed/untrusted messages cannot crash BVE or cause unbounded allocation.
- Configuration has an explicit schema version and migration/rejection policy.
- The plugin can be disabled or disconnected without restarting BVE where the approved API permits it.
- UI controls receive stable automation identifiers.
- All workers and handles are released when the plugin unloads or BVE exits.

## Out of scope for the first release

- Remote authentication or Internet exposure.
- Automatic updater.
- Support for frameworks other than BveEX.
- Installer packaging.
- Guaranteed single-DLL delivery when a justified dependency makes that materially worse.

## Open decisions requiring human approval

1. Exact BVE and BveEX versions supported.
2. Exact BVE values to read and commands/state to accept from external peers.
3. Wire protocol fields, byte order, encoding, framing, version negotiation, and compatibility policy.
4. UDP direction(s), addressing, cadence, packet loss behavior, and maximum packet size.
5. TCP client/server concurrency, reconnect backoff, keepalive, and message framing.
6. Serial framing, baud/parity/stop bits defaults, timeout, and hot-plug behavior.
7. Configuration location, format, migration policy, and sensitive fields.
8. Logging location, retention, verbosity, and privacy rules.
9. Whether physical serial hardware is mandatory for completion or loopback is acceptable.
10. Fixed BVE E2E scenario and the exact observable pass conditions.
11. Release artifact layout and licensing constraints of referenced BveEX assemblies.

## Product completion

The product is releasable only when all required feature specs are Approved and passed, the full
verification command succeeds from a clean checkout on the reference machine, and the final report
lists supported versions, limitations, and all retained evidence.

