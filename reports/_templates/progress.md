# Progress checkpoint template

Template only: fill actual values and remove inapplicable fields with an explicit reason.

- Updated: <timestamp with timezone>
- Goal: [goal.md](goal.md)
- State and active task: <planning | implementing | verifying | blocked | complete; task ID>
- Branch / HEAD / tested revision: <safe identifiers>
- Working-tree changes: <owned relative files, uncommitted changes, other owners>
- Attempts: <consumed / configured maximum; never reset on resume>
- Completed evidence: <observations and relative references>
- Current hypothesis / experiment: <cause, discriminating check, result or pending>
- Pending command: <safe command, start time, process/job ID, expected evidence; reconcile on resume>
- Deployment: <target alias, artifact revision/hash, actual state, backup/recovery reference>
- Shared resources: <BVE/process/port/UI/fixture owner and cleanup responsibility>
- Blockers: <reason, evidence, minimal decision/environment change>
- Next exact action: <reproducible command or bounded editing step>

## Agent ownership

| Agent | Goal | Allowed files/resources | State | Evidence / integration |
|---|---|---|---|---|
| <owner> | <bounded outcome> | <exclusive scope> | <state> | <relative references> |

## Recent commits and remaining checks

- <commit ID, purpose, relevant verification>
- <remaining acceptance checks and why not yet run>

Store raw output in ignored evidence folders; sanitize all tracked fields.
