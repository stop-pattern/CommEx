# Shared Visual Studio / VS Code workspace

Authorized scope: make the existing repository usable by a human in Visual Studio 2026 and an agent
in the VS Code integrated terminal. Preserve net48, source paths, local configuration and BVE ownership.

Acceptance: both entry files resolve the same project/root; VS solution exposes shared guidance and
scripts; VS Code tasks call repository scripts from the root; documentation explicitly assigns the
two roles; build and structural/link/privacy checks pass. Record any unavailable GUI verification.

Plan: add relative workspace/task definitions and solution folders, update linked guides and matching
README/AGENTS trees, verify structure/build/opening, review and commit as a separate infrastructure task.
No deployment, runtime feature, editor extension installation or machine-global setting change.

Owner clarification: source layout must follow Microsoft/C# conventions and be documented. Preserve
the existing conforming entry/metadata files; specify project folders, file/type naming, namespace
alignment, future responsibility folders and WinForms/test exceptions in docs/coding-standards.md.
