# CommEx build configuration

The user develops in **Visual Studio 2026**, opening root `CommEx.slnx` with File > Open > Project/Solution.
The agent works in the **VS Code integrated Windows PowerShell terminal**, opening root
`CommEx.code-workspace` with File > Open Workspace from File (or `code CommEx.code-workspace`).
Both entry points use the same checkout and SDK-style project `src/CommEx/CommEx.csproj`.
Install the .NET desktop development tools including a supported .NET SDK
and the .NET Framework 4.8 targeting pack. The runtime target remains .NET Framework, not modern .NET.

The physical folders remain `src/`, `tests/`, `scripts/`, `docs/`, `specs/`, `config/` and `reports/`.
Visual Studio solution folders expose the production project plus selected shared guides, configuration
example, scripts and test helpers. They are navigation entries; solution-item scripts are not compiled
into CommEx.dll. Use VS Code Explorer or Visual Studio's folder view for the complete physical tree.
Add future C# projects beneath their responsibility folder and reference them from CommEx.slnx.
Follow the [C# source layout rules](coding-standards.md#ソースコードのフォルダ構成) for project folders,
PascalCase file/type names and corresponding namespaces. The existing entry type and assembly metadata
already follow the documented layout.

The committed VS Code workspace opens the repository root, sets the integrated terminal to that root,
and uses Windows PowerShell without a profile. Terminal > Run Task provides setup, Debug/Release
builds and the configured DLL contract check; Ctrl+Shift+B runs the Debug build. Tasks call the same
repository scripts with explicit process arguments, including when the checkout path contains spaces.
The contract task reads `pluginOutputPath` (normally Release); a Debug build does not select Debug for
that check. Build Release first when validating the default configured artifact.
Opening the folder alone also permits terminal work, but does not load the tasks/settings in the
workspace file. No VS Code C# extension is required for this terminal-based agent workflow.

Complete local configuration/setup below before the first IDE build. Visual Studio and the agent share
source and output directories: save and coordinate edits, and run only one build at a time. Keep personal
IDE state in ignored `.vs/`, `.vscode/` or user settings; the tracked workspace contains portable shared
settings only. Build tasks do not deploy or launch BVE.

Workspace/task configuration follows the [VS Code workspace documentation](https://code.visualstudio.com/docs/editing/workspaces/multi-root-workspaces)
and [task documentation](https://code.visualstudio.com/docs/debugtest/tasks). Solution folders follow
Microsoft's [SLNX schema](https://github.com/microsoft/vs-solutionpersistence/blob/main/src/Microsoft.VisualStudio.SolutionPersistence/Serializer/Xml/Slnx.xsd).

| Setting | Value and purpose |
|---|---|
| SDK / target | Microsoft.NET.Sdk / net48, with UseWindowsForms enabled |
| Output | Library named CommEx.dll; root namespace CommEx |
| Architecture | AnyCPU, Prefer32Bit=false; host-specific compatibility remains unverified |
| Language | C# 7.3, without latest/preview language selection |
| Assembly metadata | Properties/AssemblyInfo.cs remains authoritative; GenerateAssemblyInfo=false prevents duplicates |
| Version / COM | Existing 1.0.0.0 assembly/file versions, GUID and ComVisible(false) are preserved |
| Artifacts | bin/Debug or bin/Release under src/CommEx; no target-framework suffix |
| Symbols / docs | Existing full Debug and pdbonly Release symbols; XML documentation beside the DLL |
| Compilation | Deterministic output; SDK includes source/resource files automatically |
| Host reference | Installed BveEx.PluginHost.dll via BveExRuntimeDirectory; Private=false (no copy-local) |

Before opening the solution for development, configure `config/repo.local.json` using the tracked
example without overwriting existing machine settings. Set `msbuildPath`, `vstestPath`, `bveExecutable`
and `bveExExtensionDirectory` to the authorized installed tools/host paths, then run:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/setup-dev.ps1
```

The setup validates those paths and the BveEX 2 PluginHost assembly identity. It uses the configured
Extensions directory's parent as `BveExRuntimeDirectory` and writes ignored
`src/CommEx/CommEx.local.props`, imported by the project for IDE and CLI builds. Rerun setup and reload
the IDE project after changing the installation path. Alternatively, an explicitly supplied MSBuild
`BveExRuntimeDirectory` property can select the installed reference. The project fails with a setup
instruction if that reference is missing. Do not commit local props or copy the installed host binaries
into the repository or build output.

Run `powershell -ExecutionPolicy Bypass -File scripts/build.ps1` (or add `-Clean` for rebuild).
Use `-Configuration Debug` or `-Configuration Release` to override the local configuration for a single build.
The script uses configured Visual Studio MSBuild and performs restore before build. Set local configuration
`solutionPath` to `CommEx.slnx`; the Release `pluginOutputPath` is `src/CommEx/bin/Release/CommEx.dll`.
Retain other machine settings when updating these paths.

The library contains `CommEx.CommExMain`, the resource-free Extension defined by the approved
[configured-host bootstrap](../specs/001-host-bootstrap/spec.md). It uses `AssemblyPluginBase`,
`Plugin(PluginType.Extension)` and `IExtension`; constructor, Tick and Dispose add no I/O, workers,
events, UI or communication behavior. BveEX discovers the attributed type in its Extensions directory.
The assembly description is `CommEx development bootstrap (no communication features)` and its version
is 1.0.0.0. Framework references remain in place and the desktop SDK supplies WinForms/Drawing.

The configured BveEX 2.1.51225.1 installation provides PluginHost assembly version 2.0.50204.1;
the host product and assembly versions are distinct. The project references the installed assembly with
`Private=false`, and setup does not redistribute host binaries. AtsEX/legacy-mode references,
host-specific release artifacts and distribution/license verification remain
[B09/P010 work](../specs/000-product/tasks.md). No assembly merging, signing key, post-build deployment
or automatic host launch is introduced. The entry contract follows the
[official BveEX extension quickstart](https://bveex.okaoka-depot.com/wiki/quickstart).

After a Release build, run the offline contract test against `pluginOutputPath`:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/test-bootstrap.ps1
```

It checks the net48 assembly's entry type/attribute/constructor and absence of copied host dependencies.
It does not start BveTs or prove the host instantiated the plugin. Actual host load, artifact identity,
responsiveness and graceful exit require the separate [bootstrap acceptance](../specs/001-host-bootstrap/spec.md)
and retained evidence; task status is in the [bootstrap task list](../specs/001-host-bootstrap/tasks.md).

For the configured BVE 6 installation, set `bveWorkingDirectory`, `bveProcessName`, `e2eEnabled=true`
and `bveExStatePath` in local configuration. The state path must be `LoadedExtensions.xml` beside the
Extensions directory; it stores extension toggles, not registration. Install the `winapp` CLI for UI checks.
With BveTs closed, deploy the verified Release artifact and run the real-host check:

```powershell
$bootstrapHash = (Get-FileHash src/CommEx/bin/Release/CommEx.dll -Algorithm SHA256).Hash
powershell -ExecutionPolicy Bypass -File scripts/deploy-bootstrap.ps1 -ExpectedSha256 $bootstrapHash
powershell -ExecutionPolicy Bypass -File scripts/test-bve-bootstrap.ps1 -ShowWindow
```

Deployment checks the entry contract and hash, backs up the installed DLL and available toggle-state
file under ignored `artifacts/`, and verifies the copied DLL. Keep backups outside Extensions because
BveEX scans its subdirectories too. The host check opens the plugin list without selecting a scenario,
asserts the new version/description, verifies the exact DLL file mapping and hash, saves a screenshot,
checks responsiveness and closes its own host normally. It refuses to launch alongside an existing
BveTs. Only attach to an explicitly authorized process with `-HostProcessId <PID> -CloseAfterTest`;
that option also closes the process. A user-requested restart after verification may remain open.

To restore a pre-existing DLL after closing BveTs, use the deployment evidence directory:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/restore-bootstrap.ps1 -EvidenceDirectory artifacts/<timestamp>-bootstrap-deploy
```

Restoration verifies the manifest target, current deployment hash and backup hash before copying.
This helper restores an existing prior DLL; it does not uninstall a fresh installation or reset toggle
states. The saved state file remains available for explicit recovery. Raw evidence contains local paths
and must remain ignored. The verified bootstrap outcome is recorded in the
[bootstrap report](../reports/minimal-host-bootstrap/summary.md).

Build/contract success does not certify the product. Application unit/transport integration assemblies
and the full six-host E2E implementation are still missing; `scripts/test-bve.ps1` remains a deliberate
failure placeholder and `scripts/verify.ps1` must retain the broader failures.
The bootstrap does not complete F001 or implement communication features. The earlier project conversion
is recorded in the [migration evidence](../reports/sdk-style-migration/summary.md).

SDK behavior references: [Microsoft SDK properties](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props)
and [Microsoft desktop migration guidance](https://devblogs.microsoft.com/dotnet/how-to-port-desktop-applications-to-net-core-3-0/).
