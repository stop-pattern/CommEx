# CommEx build configuration

Open `CommEx.slnx` in Visual Studio 2026. It contains the SDK-style project
`src/CommEx/CommEx.csproj`. Install the .NET desktop development tools including a supported .NET SDK
and the .NET Framework 4.8 targeting pack. The runtime target remains .NET Framework, not modern .NET.

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

Run `powershell -ExecutionPolicy Bypass -File scripts/build.ps1` (or add `-Clean` for rebuild).
Use `-Configuration Debug` or `-Configuration Release` to override the local configuration for a single build.
The script uses configured Visual Studio MSBuild and performs restore before build. Set local configuration
`solutionPath` to `CommEx.slnx`; the Release `pluginOutputPath` is `src/CommEx/bin/Release/CommEx.dll`.
Only these project paths were aligned in the existing local configuration; retain other machine settings.

The library currently contains the original scaffold only. Existing framework references are retained,
and WinForms/Drawing references are supplied by the desktop SDK. No third-party DLL/package, host entry point,
assembly merging, signing key, post-build deployment or automatic host launch is introduced.
Choosing AtsEX/BveEX reference assemblies, exact versions, copy-local policy and host-specific release
artifacts remains [B09/P010 work](../specs/000-product/tasks.md), including distribution/license verification.
Do not deploy this scaffold as a working host plugin.

Build success verifies infrastructure only. Unit/integration assemblies and the host E2E implementation
are still missing; `scripts/verify.ps1` must report their failure rather than certify the product.
See the [migration evidence](../reports/sdk-style-migration/summary.md).

SDK behavior references: [Microsoft SDK properties](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props)
and [Microsoft desktop migration guidance](https://devblogs.microsoft.com/dotnet/how-to-port-desktop-applications-to-net-core-3-0/).
