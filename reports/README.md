# Reports

Create one directory per feature, for example `reports/F004-udp-transmission/`.

Commit:

- `summary.md`: what changed, acceptance criteria, limitations, and significant debugging decisions.
- `result.json`: machine-readable result conforming to `../result.schema.json`.

Do not normally commit bulky or machine-specific evidence. Keep it locally under the ignored
`logs/`, `screenshots/`, and `test-results/` subdirectories and list its relative path in `result.json`.
For a release, archive the required evidence separately if long-term retention is needed.

