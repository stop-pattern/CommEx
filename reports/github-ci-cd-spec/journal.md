# GitHub CI/CD documentation journal

## 2026-09-17 22:03 UTC — scope and inspection

- Owner requested documentation for GitHub management and CI/CD DLL downloads: push checks/build/upload with retention <= one day; tag push runs the same pipeline then creates a draft release with attachments.
- Initial request mentioned three workflows; requested missing third trigger/actions asynchronously. Do not infer automatic publication or a PR workflow.
- Inspected branch/status, product contracts, constitution, development workflow and SDK migration checkpoint. Base 715e54f has unrelated uncommitted SDK migration work; preserve it.
- Delegated independent read-only requirements review to ci_requirements_review. Coordinator exclusively owns edits and index.
- Official references checked: upload-artifact retention minimum is one day; GitHub supports draft releases and uploaded release assets. Sources are linked in the new specification.
- Commands: git status --short, git branch --show-current, git log -3 --oneline and document reads exited 0. No feature build/test/deployment attempted.
- Next: document the two confirmed workflows, blockers and acceptance; validate and commit documentation independently.

## 2026-09-18 JST — clarification and review integration

- Owner explicitly corrected the workflow count from three to two. Removed third-workflow placeholders from current specifications and tasks; the earlier journal entry records the original clarification state only.
- Read-only review confirmed the need to distinguish draft preparation from published product distribution, preserve B09 licensing and use runner-local build configuration. Incorporated these points.
- Added CI-01/CI-02, CI-AC-01 through CI-AC-05 and unchecked CI001 through CI004, with references from README/spec/acceptance/plan/tasks. No workflow YAML or remote resource changed.
- Next: link/content/whitespace/privacy verification and selective staging that excludes existing SDK migration edits.

## 2026-09-18 JST — documentation check refinement

- Initial documentation checker exited 1: its drive-path expression matched the final letter of an HTTPS URL. Inspection identified a checker false positive, not a personal filesystem path. Added the missing word boundary; retained the initial result in artifacts/github-ci-cd-spec/documentation-check-initial.json.
- git diff --check exited 0. Existing CRLF normalization warnings are unrelated to the content checks.
- Re-run the corrected checker before committing; this documentation check is not plugin feature verification.
