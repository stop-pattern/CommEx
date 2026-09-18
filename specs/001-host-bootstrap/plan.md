# Minimal host bootstrap implementation plan

Goal: satisfy [BOOT-01 through BOOT-05](spec.md) for the configured host without implying product completion.
Architecture: a resource-free Extension class, local reference setup, an offline loader-contract test and a separately observable real-host load test. Coordinator serializes deployment and UI resources in the clean existing develop checkout; no competing implementation owner.

## Task 1 — compile a host-loadable DLL

Files: src/CommEx/CommEx.csproj, src/CommEx/CommExMain.cs, assembly metadata, scripts/setup-dev.ps1, scripts/test-bootstrap.ps1, tests/Integration/bootstrap-contract.ps1, .gitignore.

1. Write a reflection-based test for the official Extension contract and run it against the existing Class1 scaffold; require failure for missing CommEx.CommExMain.
2. Resolve the installed PluginHost directory from configured Extensions parent; validate tools and produce ignored CommEx.local.props for MSBuild/IDE.
3. Replace Class1 with the minimal documented Extension and disable copy-local on the host reference. Add a clear missing-reference build error.
4. Run repository Release/Debug builds and the entry-contract test; review XML docs and artifact dependencies. Commit the coherent implementation/test/setup increment when these checks pass.

## Task 2 — deployment and actual load evidence

Files: scripts/deploy-bootstrap.ps1, scripts/test-bve-bootstrap.ps1, tests/BveE2E bootstrap assertions, ignored artifacts/minimal-host-bootstrap, local configuration only if a further external write target is required.

1. Validate no existing BveTs process and source contract/hash; save prior DLL/toggle-state file before copying, compare destination hash and retain manifest.
2. Launch the configured host; use winapp UI observations to identify stable controls. Capture BveEX instantiated-plugin details and process DLL identity; load the declared scenario if needed.
3. Verify host responsiveness, gracefully close only the owned process and retain shutdown result. Preserve failure evidence before repairing any issue within the five-attempt budget.
4. Make the successful observations reproducible through a repository script, including failure when plugin/version/identity evidence is absent. Commit this verification increment after its checks pass.

## Task 3 — integration and handoff

Update README, build instructions, task register and reports. Run scripts/verify.ps1, retain broader missing-test failures, and distinguish the completed bootstrap evidence from the unfinished six-host F001 feature. Check documentation/privacy/whitespace, record tested commits and recovery material, then commit the evidence/documentation increment.
