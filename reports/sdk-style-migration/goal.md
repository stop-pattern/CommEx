# SDK-style library migration

User-authorized scope: convert the existing Visual Studio class library to SDK style and configure its DLL build settings.
Preserve .NET Framework 4.8, assembly identity/version and AnyCPU; enable WinForms, retain existing framework references,
place production sources under src/CommEx, and align the solution, configuration example and build entry point.
No deployment or implementation of host/transport features is authorized by this infrastructure change.

Acceptance: repository build script produces CommEx.dll for net48; inspect assembly identity/framework/architecture,
verify Debug and Release output and review solution/configuration consistency. Run verify.ps1 and report existing
unavailable feature gates without changing them into passing tests. Preserve raw evidence under ignored artifacts/.

Sources: [product plan](../../specs/000-product/plan.md), [tasks](../../specs/000-product/tasks.md),
[acceptance](../../specs/000-product/acceptance.md). B09 still blocks host-specific dependency/artifact selection.
