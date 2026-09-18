# Minimal host bootstrap goal

- Objective: prepare coding prerequisites, create the smallest CommEx host plugin and verify that the configured BveTs actually loads it.
- Authority: owner's 2026-09-18 request authorizes this bounded bootstrap implementation and configured-host verification. This is an increment toward F001, not completion of the six-host product feature.
- Scope: installed configured BVE/BveEX environment, minimal net48 Extension entry point, reproducible build/reference setup, safe deployment and machine-verifiable load evidence.
- Acceptance: repository build succeeds; required host entry contract is validated; configured BveTs loads the exact built DLL and instantiates the plugin; host remains responsive and can shut down cleanly; retain evidence and commit by task.
- Constraints: no protocol/UI feature scope expansion; no I/O waits in host callbacks; no bundled host binaries; only configured external targets; preserve recovery material before changing installed files.
- Plan: inspect local runtime and official API; specify the bootstrap increment and tests; implement minimum entry point; build/test; safely deploy/run/load-check/cleanup; run aggregate verification and report broader limits accurately.
- Attempt budget: maximumFeatureAttempts=5; count in progress.md across interruptions.
