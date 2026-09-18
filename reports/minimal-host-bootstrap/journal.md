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

## 2026-09-18 JST — bootstrap implementation committed

- Independent API/code review found no material issues. Documentation review checked 62 local links and matching README/AGENTS trees; coordinator reviewed integrated diffs and staged whitespace passed.
- Commit 2b7042a contains only BOOT-T01 source/setup/tests/specification and corresponding docs. Deployment helpers/config field remain a separate uncommitted BOOT-T02 increment.
- Owner authorizes future git add -- staging without repeated conversational confirmation. Sandbox still requires the tool's permitted write mechanism for Git metadata.
- Next authorized external action: back up previous DLL/toggle states outside Extensions, deploy the verified Release hash, then launch the configured BveTs under coordinator ownership.

## 2026-09-18 JST — deployment and unsuccessful initial launch

- deploy-bootstrap.ps1 exited 0; saved previous DLL/state in artifacts/20260918-105620-bootstrap-deploy and verified new destination hash.
- Started configured BveTs with hidden startup, owned PID 25076. Initial process responded but had no main window and no BveEX/CommEx modules. UI inspection returned no windows; retry outside sandbox also found no matching application.
- Subsequent process inspection confirms PID 25076 exited. The initial sandbox-only UI visibility hypothesis is not established; investigate actual startup/exit before blaming plugin code. Raw startup-ui.json, startup-ui-desktop.json, host-process.json and startup-events.json retained under artifacts/minimal-host-bootstrap.
- No product/source change made in response. Next: inspect startup event evidence and record exit behavior in a controlled owned-process launch.

## 2026-09-18 JST — owner clarification and actual load observation

- Owner clarified that initial BveTs was accidentally closed, then requested restart. This explains the prior exit; no plugin startup defect was established. Restarted configured executable as owned PID 40952 with a normal visible window.
- BveTs opened ScenarioForm. Escape did not dismiss it; invoking its observed Close button did. Right-clicking the observed MainForm followed immediately by invoking the BveEX version/plugin-list item opened the list; separating those operations allowed menu focus loss, so the eventual harness must keep them together.
- Live UI evidence version-dialog-ui.json shows instantiated CommEx.dll, name CommEx, version 1.0.0.0 and description CommEx development bootstrap (no communication features). Host UI still responds. No scenario is needed for this extension-list check.
- Native Process.Modules did not list managed plugin DLLs. Use independent process file-mapping evidence for exact DLL path rather than claiming that API succeeded.

## 2026-09-18 JST — reproducible host assertions

- Added saved-UI assertions and a real-host runner. Correct snapshot passes; altered previous-plugin description fails at column 4 as intended. Initial Japanese literal mismatch came from Windows PowerShell script decoding; use observed BveEX title prefix plus exact plugin cells instead of locale-dependent literal parsing.
- Read-only process file-map probe compiled after avoiding nameof unsupported by the PowerShell Add-Type compiler; production target remains C# 7.3/net48. Initial probe hit native error 1006 on a pagefile-backed mapped region, so narrowing that known no-file case while retaining fatal treatment of other inspection errors. Probe still requires an exact deployed-path match.
- Cycle 2/5 covers host automation/probe refinement. No plugin source repair required; raw UI evidence already proves construction.

## 2026-09-18 JST — successful host assertions and fresh-launch regression

- Process map positive/negative controls passed: deployed CommEx path mapped=true, missing DLL mapped=false. Runner on owned PID40952 exited0, exact UI cells/mapping/hash/responding/screenshot passed, then shutdownPassed=true. Evidence: artifacts/20260918-112307-bootstrap-host/result.json and plugin-list.png; screenshot visually inspected.
- Aggregate verify.ps1 exited1 at missing unit-test assembly after build passed; artifacts/minimal-host-bootstrap/aggregate-verify.{json,log}. Full F001 remains incomplete.
- Fresh runner failed: nested ScenarioForm was present under MainForm, but top-level-only detection missed it. Saved ui-09/ui-13 show disabled MainForm. Corrective change: traverse nested elements, resolve actual scenario HWND, and forbid clicking disabled MainForm.
- Fresh-run evidence: artifacts/20260918-112412-bootstrap-host/result.json. After its failure, host restarted into a legacy/sample scenario as PID36988; CIM confirms parent PID39040 belongs to this verification run. Preserve screenshot/UI error evidence, close only that owned descendant before retry. Cycle3/5.

## 2026-09-18 JST — readiness guard and recovery verification

- Aborted owned descendant's sample load and closed it normally (both closeRequested/exited true); no remaining BveTs then.
- Aggregate rebuild changed the DLL hash; entry source unchanged, generated SourceLink now identifies commit2b7042a. Stale-deployment hash guard correctly rejected the next run before launch. restore-bootstrap.ps1 restored the original DLL successfully; redeployment verified current hash F1BD5B56CAE955E8FF05196EED873597414602EDF9AE54254D4467CAA36E0F66. Recovery bundle artifacts/20260918-113403-bootstrap-deploy.
- Fresh-run evidence artifacts/20260918-113404-bootstrap-host: initial main-window snapshot preceded ScenarioForm creation; later snapshot had modal-disabled MainForm. The new guard safely refused mouse action; no unintended scenario selection. Owned PID43520 remains for cleanup.
- Cycle4/5 refines startup readiness: wait up to60seconds for an owned startup dialog on fresh launches before traversing UI. No plugin behavior changed.

## 2026-09-18 JST - final host result and requested restart

- Attached run on PID43520 passed and closed normally: artifacts/20260918-113606-bootstrap-host. Review fixes enforce canonical state path, attached-host shutdown authorization, zero exit status and bounded UI CLI calls; record input hashes.
- Fresh runner passed: artifacts/20260918-113756-bootstrap-host/result.json, 02:37:56Z through 02:38:13Z. Exact metadata/mapping/hash, responsiveness and graceful exit passed for PID33960. No fifth cycle required. The pending invocation is reconciled; no verification host remained.
- Owner requested another restart. Started configured BveTs visibly at 11:51 JST, PID36880. winapp list-windows confirms scenario-selection and main windows. Leave this interactive instance open; do not assume test ownership.
- Rechecked offline contract (exit0), successful evidence input hashes (all five match), and whitespace. Read-only review by bootstrap_docs found no material blockers. Prior-DLL restoration scope and explicit toggle recovery are documented.
- Final records distinguish passing BOOT acceptance from aggregate unit-stage failure. Coordinator owns scoped helper/evidence commits; no concurrent writer.
- Final checks passed: current result.schema.json constraints (local recursive validation, no third-party package installed), 41 local documentation links, README/AGENTS tree equality, PowerShell syntax and git diff --check. No further build performed after deployment, preserving tested artifact identity. The optional jsonschema module was unavailable; no dependency was added for record validation.
