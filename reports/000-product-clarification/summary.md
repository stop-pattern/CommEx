# Product clarification documentation update

Date: 2026-09-18

Scope: Implement the approved documentation-update plan. This is not a plugin feature implementation
or a feature completion report. No feature task was marked complete, and no feature result.json was
created or represented as passing.

## Recorded decisions and changes

- Recorded 28 normalized clarification decisions and 19 functional requirements in the product spec.
- Preserved F001–F010 and added F011–F016 for legacy codecs, public data/input, Native JSON, MQTT,
  WebSocket and profiles/automatic connections; all feature tasks remain unchecked.
- Added the six required host combinations, transport/format compatibility matrix, initial public
  data/write inventory and 14 product acceptance criteria.
- Recorded automatic connection/reconnect checkboxes, finite retries, whole-profile precedence,
  scenario reset and valid configuration retention on endpoint connection failure.
- Recorded minimal-DLL distribution preferences, physical Serial loopback and performance thresholds.
- Updated README and annotated the original research memo so its Native binary proposal and initial
  prioritization cannot override the later decisions.
- Kept B01–B10 explicit: detailed contracts, exhaustive host API mapping, bounds, configuration/UI
  semantics, environment/workload evidence, dependencies/licenses and diagnostic details still need closure.
- Recorded the owner's standing instruction to commit each smallest meaningful verified increment
  in AGENTS.md, with matching guidance in README and the product plan. Feature completion gates remain unchanged.

Local commits preceding this evidence update:

- ea71efe: agent instructions for frequent coherent incremental commits.
- f303344: mutually dependent product specifications, roadmap, task register and reference documentation.

The verification record is a separate evidence-only commit; interdependent product documents were
kept together so the versioned specification, references and task IDs remain consistent.

Main entry points: [specification](../../specs/000-product/spec.md),
[plan](../../specs/000-product/plan.md), [tasks](../../specs/000-product/tasks.md),
[compatibility](../../specs/000-product/compatibility.md),
[data catalog](../../specs/000-product/data-catalog.md),
[acceptance](../../specs/000-product/acceptance.md).

## Verification evidence

Tested documentation commit: f303344196ed5ee25fd6f33ab08d2117da8c2015.
The eight checked documents match that commit. Exact working-copy SHA-256 hashes and UTC check time
are retained in [documentation-checks.json](documentation-checks.json); this file is intentionally a
documentation-specific record, not a feature result conforming to reports/result.schema.json.

Final documentation checks: **95/95 passed**, covering eight documents and 34 local link occurrences.
Checks cover IDs/counts, feature/acceptance references, six baseline environments, unchecked tasks,
key agreed defaults, code fences, absence of machine-local absolute paths, intended tracked scope and
git diff --check. The earlier pre-commit run passed 100 checks, including five additional assertions
about tracked files then being modified; the committed-tree run has no such modified files.
Human review compared the documents with the approved update plan and the dialogue;
these structural checks do not establish wire compatibility or exhaustive API coverage.

Retained local check command:

```powershell
powershell -ExecutionPolicy Bypass -File reports/000-product-clarification/logs/check-documentation.ps1
```

The check script and raw logs are retained locally in the ignored logs directory. The first check
attempt exposed two check-tool defects: Windows PowerShell read a BOM-less script containing a
Japanese filename using code page 932, and a path regex matched the end of an HTTPS scheme. The
reproduction confirmed both causes. UTF-8 BOM and a drive-letter boundary corrected the checks;
the final run passed without modifying the product requirements to satisfy them.

The canonical repository verification command was also run during the documentation update on base
commit d031e3ed4f5e428a5c8c13277bfb3e6b9f0643fa, without skipping BVE E2E:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/verify.ps1
```

Result: **exit 1**. The configured CommEx.sln does not exist, so the build stage stopped before MSBuild.
Unit, integration and BVE E2E did not run. Raw evidence: logs/verify.log. This is the current
implementation/infrastructure blocker, not a successful build and not a reason to broaden this
documentation task into application implementation.

No sources, build/test scripts, runtime configuration, dependencies, constitution or third-party
assets were changed. No application test, physical Serial test or six-environment E2E success is claimed.
