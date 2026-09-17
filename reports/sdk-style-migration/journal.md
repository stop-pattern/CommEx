# Action journal

## 2026-09-18 — initial assessment

Inspected branch/HEAD/status, legacy project, AssemblyInfo, solution, repository scripts and product plan/tasks/acceptance.
Initial tree clean at 715e54fa0b5eed33aac20ca03aec6660d253daf3. User requested SDK conversion and DLL settings.
The existing project contains only Class1, has no third-party references, targets net48 and retains version 1.0.0.0.
Solution/local configuration and canonical source location disagree. This is an explicitly authorized infrastructure
increment, not F001 host implementation. B09 prevents selecting host-specific dependencies without a decision.
Plan recorded before implementation; raw logs will stay under ignored artifacts/.

## 2026-09-18 — migration and review

Agent sdk_review performed read-only migration/scope analysis (no files/resources modified). Integrated its recommendations:
preserve assembly metadata/debug formats/AnyCPU; enable restore; clarify scaffold versus B09; document Class1.
Moved tracked sources to src/CommEx through explicit file patches; old ignored bin/obj remain untouched.
Converted to net48 SDK library with WinForms, C# 7.3, deterministic build, XML docs and stable output path.
Aligned slnx and config/example paths; added optional build Configuration override for Debug/Release verification.
Updated README/AGENTS tree, coding guide and product plan/tasks. No runtime dependency was added.
Attempt 1/5 started. Pending repository builds and artifact/whole-verification checks; raw evidence in artifacts/sdk-style-migration.

## 2026-09-18T06:47+09:00 — first build failure

Release rebuild exited 1 after successful restore. Roslyn reported MSB3883: duplicate environment keys Path/PATH.
Evidence: artifacts/sdk-style-migration/release-build.{log,json}. This precedes C# compilation diagnostics.
Hypothesis: sandbox process environment contains case-colliding PATH keys, rather than a project/target failure.
Discriminating experiment: rerun the unchanged repository command outside the sandbox; preserve both outputs.

## 2026-09-18 — Release build verified

Unchanged `powershell -ExecutionPolicy Bypass -File scripts/build.ps1 -Clean` outside sandbox exited 0.
Evidence: artifacts/sdk-style-migration/release-build-outside.{log,json}. This supports the sandbox-environment
hypothesis; no project fix or compiler downgrade was needed. MSBuild 18.10.1.42706, selected SDK 10.0.401;
Framework 4.8 targeting pack exists. Pending Debug rebuild and whole verification outside sandbox.

## 2026-09-18 — Debug and aggregate verification

Debug clean build exited 0. verify.ps1 exited 1 at test-unit.ps1 because no unit-test assembly exists; its build stage
passed. Integration/BVE stages were not reached; their missing implementations remain explicit. Evidence:
artifacts/sdk-style-migration/debug-build.{log,json}, verify.{log,json}. No change was made to these gates.
Agent sdk_review reviewed the final diff and found no concrete defects; requested completion of evidence/summary.
Prepared ignored artifact checker for both generated assemblies (framework, identity, MSIL, COM GUID/visibility,
file version, library entry point, retained public type, symbols/XML docs) and local path/tree/link consistency.

## 2026-09-18 — artifact helper correction

The first artifact audit exited 1 because reflection-only loading both assemblies with identical identity in one
AppDomain is prohibited. This was an audit-helper limitation, not a product build failure. Changed only the ignored
helper to accept one configuration per invocation, isolating Debug and Release in separate PowerShell processes.
The same metadata assertions are retained. No product implementation repair or attempt-budget reset occurred.

## 2026-09-18T07:07+09:00 — artifact verification and concurrent changes

Both separate artifact checker invocations exited 0. Verified net48/MSIL/version/COM/library metadata, preserved
public type, PDB/XML, configuration and matching folder trees; 48 local links and git diff --check passed.
Sanitized validation.json records artifact hashes and links command evidence. Feature completion remains false.
Detected concurrent CI/CD documentation work in README, product plan/tasks/spec/acceptance and its new report/spec.
Those edits are unrelated and preserved. Its progress record says the other coordinator owns the Git index and is
preparing a scoped commit. Do not stage or commit until that operation has finished; then recheck HEAD/index and
commit only SDK migration files/hunks. No writes to the other work record.

## 2026-09-18 — handoff

Rechecked HEAD/index and the other coordinator's checkpoint after a bounded wait: HEAD unchanged, index empty,
but index ownership still explicitly assigned to its pending commit. Did not infer that an empty index releases
ownership. No migration staging/commit attempted. Saved a coherent progress snapshot with next exact action;
no background task remains. SDK build settings are implemented and verified; aggregate feature verification and
host dependencies remain blocked as described above, and the local migration commit awaits resource coordination.

## 2026-09-18 — scoped commit resumption

User explicitly requested small meaningful commits. CI documentation and task-commit-policy work are now committed
through 5e407a8; the other checkpoint releases ownership and the index is empty. Only migration edits remain.
Plan: independent build-script capability, coherent SDK/layout/docs migration, then final commit-bound evidence.
Fresh source-hash comparison passed for all six recorded build inputs; both artifact checker invocations passed.
No code changed since successful Debug/Release builds, so do not repeat those builds without a new reason.
Configured Git identity matches the owner-authorized identity recorded for be885f0; retain it without overrides.

## 2026-09-18 — build infrastructure commit

Sandbox staging failed because the index is read-only; approved external staging succeeded. Inspected the staged
script diff and whitespace check. Committed only scripts/build.ps1 as 138048c, adding restore and the per-invocation
configuration override. Pending: project migration and its related documentation/records, followed by final evidence.
