# Bootstrap action journal

## 2026-09-18 JST — start and reconciliation

- New owner request: establish development environment, create minimum plugin, prove BveTs loads it.
- HEAD c797114 contains completed SDK migration; initial git status was clean. Existing script placeholders are not passing test infrastructure.
- Configured BVE/BveEX/scenario/MSBuild/vstest paths exist; no configured BveTs process was observed. Raw machine paths are kept out of tracked records.
- Delegated read-only API/reference/licensing/load-evidence research to host_api_research; coordinator exclusively owns runtime/deployment and code edits.
- Read build/deployment/test/verify scripts, constitution and prior SDK migration records. Deployment helper currently lacks backups/ownership checks; host test is a deliberate full-feature failure placeholder.
- Next: identify exact host entry contract and minimal observable lifecycle, then persist spec/plan/tasks before implementation.

## 2026-09-18 JST — contract and first implementation cycle

- Configured BveEX is 2.1.51225.1, PluginHost assembly 2.0.50204.1. Existing registration selects CommEx.CommExMain; previous installed CommEx is 1.11.9590.6850, SHA256 5CC0064BEB1B83E99846984EC8569A13C22628F0D43DDC8778CDB1EE04D41C2A.
- Wrote bounded spec/plan/tasks under specs/001-host-bootstrap. Work remains in the clean existing checkout with exclusive coordinator ownership and scoped commits, consistent with the owner's task coordination instructions.
- powershell -NoProfile -ExecutionPolicy Bypass -File scripts/test-bootstrap.ps1 exited 1 before implementation: missing required CommEx.CommExMain. This discriminates the original compilable scaffold from a host-loadable Extension.
- Added the official resource-free Extension entry point, local host-reference setup and copy-local prohibition. Cycle 1/5; pending repository builds and positive contract check.

## 2026-09-18 JST — build checks and loader clarification

- setup-dev.ps1 exited 0. Initial Release build exited 1 with MSB3883 from duplicate Path/PATH in sandbox; retained artifacts/20260918-104626-build/msbuild.log. Unchanged Release build outside sandbox exited 0; subsequent Debug build exited 0. test-bootstrap.ps1 exited 0 with the required net48 Extension contract and no copied host assemblies.
- Official loader research corrects the earlier registration assumption: LoadedExtensions.xml only restores/saves togglable extension states. Plugin DLL/type discovery uses attributes. Host plugin-list rows consume successfully created PluginBase instances, allowing UI evidence of construction.
- Delegate documentation alignment to bootstrap_docs with exclusive write scope over README/AGENTS/build/coding guides and product plan/tasks; coordinator continues deployment/test tooling. No resource/working-tree conflict.

## 2026-09-18 JST — deployment preflight

- Added separate bootstrap deployment/restoration helpers. Backups are outside Extensions because the host scans DLLs recursively; host-state path declared in ignored local config before launch.
- Incorrect ExpectedSha256 test exited 1 with the intended source-hash rejection before any backup or external write. PowerShell parser checks passed for both helpers. Actual deploy/restore validation remains pending.
- Reference setup is ignored by Git. Release/Debug artifacts exist and the normal build uses only the installed PluginHost reference with copy-local disabled.
- Raw combined check record: artifacts/minimal-host-bootstrap/build-checks.json. Successful build logs: artifacts/20260918-104716-build/msbuild.log and artifacts/20260918-104734-build/msbuild.log.
