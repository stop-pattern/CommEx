# Workspace journal

## 2026-09-18 - scope and inspection

- Existing root CommEx.slnx refers to src/CommEx/CommEx.csproj; no source relocation is needed.
- VS Code is installed. Root *.code-workspace is ignored; add a narrow tracked workspace exception.
- Chose a single relative-root workspace with manual process tasks, keeping local .vscode settings ignored.
- Consulted Microsoft SLNX schema and VS Code workspace/task documentation. Solution folders are logical
  views; all file paths remain relative to the solution root. User explicitly assigns Visual Studio 2026
  to human development and VS Code integrated terminal to agent operations.
- Delegated read-only consistency review to ide_layout_review; coordinator owns files, index and verification.

## 2026-09-18 - verification

- Debug build through scripts/build.ps1 exited0; artifacts/20260918-121812-build/msbuild.log.
- Structure check passed workspace/task paths, solution references, matching trees, ignore exception and 45 links.
- dotnet solution listing initially hit sandbox first-use denial. Authorized retry exited0 and listed
  src/CommEx/CommEx.csproj. .NET CLI also performed its standard first-use initialization.
- Launched verification Visual Studio PID30880 and a separate VS Code workspace window (HWND2232584).
  VS Code title confirms the CommEx workspace opened. Visual Studio DTE18 reports expected solution open,
  but initial recursive COM enumeration returned zero projects; preserve visual-studio-load.json and
  inspect collection Count/Item before attributing this to a project-load error. No source/config repair yet.
- Owner added Microsoft/C# source-layout documentation requirement. Added project/folder/namespace/type-file
  rules and WinForms/test exceptions; current CommExMain and AssemblyInfo already conform, so no source move.
- Closed owned hidden VS PID30880 normally and opened interactive VS PID44344. Direct solution-tree UI
  inspection confirms src/CommEx contains Properties, dependencies, CommEx.local.props and CommExMain.cs.
  COM enumeration was insufficient evidence, not a demonstrated source/project loading defect. Saved
  visual-studio-project.json and vscode-workspace.json under artifacts/dual-ide-layout. Both editors remain
  open for the user; existing editor windows and running BVE were untouched.
- Independent final review found no material issues. No .NET target, source namespace, dependency or product
  behavior changed. The configured bootstrap-contract task explicitly identifies pluginOutputPath rather
  than implying it checks the most recent Debug build.
- Final validation passed 53 local links, workspace/solution paths, matching trees, ignore rules,
  privacy patterns and staged whitespace. Committed the coherent editor/layout task as d421b38.
  This evidence follow-up records the commit; no additional source, build or host change.
