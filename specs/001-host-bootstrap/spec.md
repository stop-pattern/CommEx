# Configured-host development bootstrap

Status: scope approved by the owner's 2026-09-18 request to prepare coding and prove a minimum plugin is loaded by BveTs. This is a bounded F001 increment, not approval/completion of all F001 behavior.

## Contract

- Target the existing configured BVE 6 / normal BveEX installation, .NET Framework 4.8 and C# 7.3. No six-host compatibility claim.
- Provide `CommEx.CommExMain` as a BveEX Extension using the official `AssemblyPluginBase`, `Plugin(PluginType.Extension)` and `IExtension` contract. BveEX discovers attributed types from DLLs in its configured Extensions directory; LoadedExtensions.xml holds toggle states, not plugin registration.
- Constructor, Tick and Dispose add no transport, file/process I/O, workers, UI or event handlers. Tick has no effect on simulation; Dispose has no owned resources to release and can repeat safely.
- Identify the build as CommEx version 1.0.0.0 with description `CommEx development bootstrap (no communication features)` so it is distinguishable from the pre-existing deployed version.
- Resolve the installed BveEx.PluginHost assembly for compilation with copy-local disabled. Do not redistribute host binaries. Generate ignored local MSBuild settings for IDE development from declared local configuration.
- Verify tools/paths before building. Before deployment require no configured BveTs process, verify the source hash/entry contract, preserve the installed DLL and extension toggle-state file as recovery evidence, copy only CommEx.dll and verify its destination hash. The host may update its toggle-state file on shutdown; declare that path in local configuration before launching it.
- Start only the configured executable, use the configured scenario if needed, and record the owned process. Verify actual instantiated-plugin visibility through BveEX UI plus the process-loaded DLL path/hash; merely copying a DLL is insufficient.
- Close the owned host gracefully after verification. Keep the verified bootstrap installed for subsequent coding; retain the previous DLL and a documented restoration command.

## Acceptance

| ID | Required observation |
|---|---|
| BOOT-01 | setup validates configured build/host paths and creates ignored IDE reference settings; repository Debug and Release builds succeed |
| BOOT-02 | automated entry-contract check rejects the previous scaffold and accepts the new net48 DLL, correct Extension type/attribute/constructor, and absence of copied host DLLs |
| BOOT-03 | deployment evidence ties source and destination hashes to the tested revision and preserves the previous installation for recovery |
| BOOT-04 | configured BveTs loads the deployed DLL; BveEX instantiated-plugin UI identifies the new CommEx version/description; raw UI/module/screenshot evidence retained |
| BOOT-05 | host responds to UI automation after loading and the owned process exits gracefully; no observed CommEx load/error dialog |

No deterministic application logic or transport is introduced, so application unit/transport integration scenarios are outside this increment; the entry-contract test and real host check are required. Run the existing aggregate scripts/verify.ps1 and record its current broader failures without weakening placeholders or declaring F001 complete.

## References

- [Official BveEX extension quickstart](https://bveex.okaoka-depot.com/wiki/quickstart)
- [BveEX source and license](https://github.com/automatic9045/BveEX)
- [Product verification](../000-product/acceptance.md) and [F001/task register](../000-product/tasks.md)
