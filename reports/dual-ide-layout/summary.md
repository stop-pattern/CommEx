# Shared IDE layout

Verified configuration/documentation commit: `d421b38`.

The user opens root `CommEx.slnx` in Visual Studio 2026. The agent opens root
`CommEx.code-workspace` in VS Code and works in its integrated Windows PowerShell terminal.
Both use `src/CommEx/CommEx.csproj`, root repository scripts and the same ignored local configuration.

The solution now groups the source project and selected documentation, scripts, configuration and test
helpers. The portable workspace defines setup, Debug/Release build and configured-DLL contract tasks.
Only CommEx.code-workspace is exempted from the existing personal workspace ignore rule.

[C# conventions](../../docs/coding-standards.md) define the Microsoft/C#-based project, folder,
namespace and type/file naming rules, including WinForms and test-helper exceptions. Existing source
files already conform; framework, language, assembly and entry-type contracts are unchanged.

Verification on base e6c021f with this task's changes:

- Repository Debug build exited 0: artifacts/20260918-121812-build/msbuild.log.
- `dotnet sln CommEx.slnx list` exited 0 and identified the existing C# project.
- Workspace JSON/task paths, solution paths, ignore scope, matching README/AGENTS trees and local links passed.
- Visual Studio 18.0 loaded the solution; direct UI inspection shows CommExMain.cs, Properties and
  dependencies under src/CommEx. Evidence: artifacts/dual-ide-layout/visual-studio-project.json.
- VS Code 1.138.0 opened the named CommEx workspace. Evidence: artifacts/dual-ide-layout/vscode-workspace.json.
- Independent review found no material issues. Both interactive editors remain available to the user.

Initial sandbox .NET CLI initialization was denied; authorized retry succeeded. Initial hidden-IDE COM
enumeration did not return a C# subproject, so actual solution-tree UI was used to establish loading.
No product repair was needed. Full product verify remains limited by missing application test assemblies
and full host harness; this infrastructure check does not complete F001. BVE/deployment were untouched.
