# Configured-host bootstrap verification

The coding environment and minimum resource-free Extension are ready. BOOT-01 through BOOT-05
passed for configured BVE 6 / normal BveEX. No communication feature or six-host compatibility
completion is claimed.

## Tested implementation

- Plugin/setup commit: `2b7042a1a8db94e1bd77e445f7b57758e28d4570`.
- Verified host helpers and development/recovery instructions: `d561bd6`.
- .NET Framework 4.8, C# 7.3, WinForms; Debug and Release repository builds passed.
- BVE 6.0.7554.619; BveEX 2.1.51225.1; PluginHost assembly 2.0.50204.1.
- Installed Release SHA256: `F1BD5B56CAE955E8FF05196EED873597414602EDF9AE54254D4467CAA36E0F66`.
- Host helpers were uncommitted during verification. All five recorded test input hashes match the
  final working files; they identify the exact tested harness independently of the source commit.

## Acceptance evidence

| Acceptance | Result / retained evidence |
|---|---|
| BOOT-01 | setup-dev passed; Release artifacts/20260918-104716-build/msbuild.log and Debug artifacts/20260918-104734-build/msbuild.log passed |
| BOOT-02 | old scaffold rejected; current test-bootstrap.ps1 passed; no copied host dependencies |
| BOOT-03 | backup, copy/hash verification, prior-DLL restoration and redeployment passed; artifacts/20260918-113403-bootstrap-deploy/deployment.json |
| BOOT-04 | CommEx.dll / CommEx / 1.0.0.0 / bootstrap description visible in instantiated-plugin UI; exact deployed DLL mapped by host |
| BOOT-05 | fresh host responded; graceful shutdown completed with exit code 0 |

Final command: `powershell -NoProfile -ExecutionPolicy Bypass -File scripts/test-bve-bootstrap.ps1 -ShowWindow`.
Fresh-run result: artifacts/20260918-113756-bootstrap-host/result.json, from 02:37:56Z to 02:38:13Z
on 2026-09-18; status passed. UI snapshots and plugin-list.png are beside that result.
The earlier attached-process run at artifacts/20260918-113606-bootstrap-host also passed for this hash.
Negative UI-description and missing-mapping controls rejected incorrect identities. Four of five
allowed cycles were used; failures remain in journal.md and ignored artifacts.

Aggregate `scripts/verify.ps1` exited 1 after a successful build because the application unit-test
assembly is absent. Integration and full BVE E2E were not reached. Command/times/log are retained in
artifacts/minimal-host-bootstrap/aggregate-verify.json and aggregate-verify.log. The accompanying
result.json records this broader F001 limit, separately from the passing bounded bootstrap acceptance.

## Development and recovery

Open `CommEx.slnx`; follow the [setup/build/host-test guide](../../docs/build-configuration.md).
Keep local props/config and host binaries untracked. The bootstrap remains installed.
The prior DLL/state backup is outside Extensions in artifacts/20260918-113403-bootstrap-deploy.
With BveTs closed, restore the prior DLL using:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/restore-bootstrap.ps1 -EvidenceDirectory artifacts/20260918-113403-bootstrap-deploy
```

Restoration was exercised successfully before the final redeployment. It restores a prior DLL only;
toggle-state recovery is explicit. Following the owner's restart request, the interactive host was
reopened and left at scenario selection. Do not deploy or take test ownership while it is in use.
