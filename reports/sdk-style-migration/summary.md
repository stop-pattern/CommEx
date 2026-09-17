# SDK-style migration outcome

The existing class-library scaffold is converted to Microsoft.NET.Sdk under src/CommEx and referenced by
CommEx.slnx. It targets net48/AnyCPU with WinForms, retains assembly metadata/version and existing framework
references, emits XML documentation and retains Debug/Release symbol formats. DLL output paths match local
configuration and its sanitized example. build.ps1 now restores before building and accepts a Configuration override.

## Verification

Base revision: 715e54fa0b5eed33aac20ca03aec6660d253daf3, develop, with migration changes applied.
Tools: Visual Studio MSBuild 18.10.1.42706, .NET SDK 10.0.401, installed .NET Framework 4.8 reference assemblies.

| Command | Result | Evidence under artifacts/sdk-style-migration/ |
|---|---|---|
| scripts/build.ps1 -Clean (sandbox) | Exit 1, compiler environment duplicate Path/PATH after successful restore | release-build.log/json |
| scripts/build.ps1 -Clean (outside sandbox) | Exit 0, Release DLL produced | release-build-outside.log/json |
| scripts/build.ps1 -Clean -Configuration Debug (outside sandbox) | Exit 0, Debug DLL produced | debug-build.log/json |
| scripts/verify.ps1 (outside sandbox) | Exit 1, build passes; unit-test assembly absent | verify.log/json |

Commands above were invoked through `powershell -ExecutionPolicy Bypass -File`.
Changing only the execution environment resolved the compiler failure; no source workaround was introduced.
Independent read-only sdk_review review found no concrete defects in the migrated project/settings/documentation.
Artifact checks passed independently for Debug and Release: net48, MSIL/AnyCPU, assembly/file version 1.0.0.0,
COM visibility/GUID, library entry point, original public type, PDB and XML documentation. Configuration paths,
matching repository trees, 48 local links and whitespace checks passed. Sanitized results: [validation.json](validation.json).
Final commit/checkpoint is recorded in progress.md. Raw helper and artifacts are retained under the evidence directory.

## Limits and remaining work

This is a build-infrastructure increment, not completion of F001 or any product feature. Required verification
remains failing; integration and BVE stages were not reached by verify.ps1. No test was weakened or skipped to pass.
The scaffold has no host entry point. Host reference DLL selection/version/copy-local/release licensing policy remains
BLOCKED on B09/P010; resolve that contract before configuring host-specific references or claiming load compatibility.
No deployment, host process manipulation or external fixture use occurred. Existing ignored output in the original
CommEx directory was preserved; the solution builds only src/CommEx.

Concurrent CI/CD documentation work completed and released Git ownership. Build-script improvements are committed
separately as 138048c; project migration and interdependent documentation are committed as a834133.
Post-commit Debug and Release artifact audits passed on a834133 with a clean working tree. All six recorded build
input hashes matched the previously successful builds, so compilation was not redundantly repeated.
Final verification/checkpoint records are retained in a separate evidence-only commit; see progress.md and validation.json.
