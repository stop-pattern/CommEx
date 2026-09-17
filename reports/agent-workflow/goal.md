# Agent workflow documentation goal

Status: complete (documentation/editor settings only)

## Objective

Document a resumable autonomous development cycle, durable progress, consistent documentation,
subagent coordination, C# conventions, an enforced repository layout, privacy and frequent coherent commits.

## Authorized scope

- Update agent/human instructions, supporting guides, report templates and the infrastructure task register.
- Add editor settings for the requested C# indentation conventions.
- Use read-only subagents for audits and a separately owned documentation/editor-settings task.
- Verify and commit each coherent increment. Do not implement or deploy the product or launch the legacy runner.

## Acceptance

- AGENTS.md and README define the same folder layout and agree with the development/report guides.
- The lifecycle covers goal, plan, implementation, build, analysis/fix, deployment, logs/debugging and verification.
- Checkpoints define actionable resume state, ownership, attempts, evidence and next steps without sensitive content.
- C# uses four spaces and XML documentation on handwritten types and members, including non-public ones.
- Privacy applies to staged contents, paths, metadata and evidence; meaningful commit boundaries are independent of prompts.
- Current runner/deployment limitations remain explicit; feature completion criteria are not weakened.
- Applicable documentation/editor-setting checks and independent review pass; changes are committed in coherent units.

## Constraints

Keep the existing .NET Framework 4.8, WinForms and approved feature requirements. Preserve the constitution
and historical evidence. Never commit personal information, local user paths or credentials.
