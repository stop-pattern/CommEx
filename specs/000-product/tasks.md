# Product task register

Only check a feature after its own approved spec exists and full required verification passes.

- [ ] F001 Extension bootstrap, lifecycle, build path, and BVE E2E smoke test
- [ ] F002 Versioned configuration load/save and recovery
- [ ] F010 Logging, diagnostic evidence, and retention limits
- [ ] F003 Lazy WinForms settings UI and automation identifiers
- [ ] F004 UDP transmission
- [ ] F005 UDP reception
- [ ] F006 TCP client
- [ ] F007 TCP server
- [ ] F008 Serial communication
- [ ] F009 Runtime configuration apply and resource replacement
- [ ] R001 Clean-checkout full regression
- [ ] R002 Release artifact and final compatibility/limitations report

## Infrastructure prerequisites

- [ ] Supported BVE/BveEX versions approved
- [ ] Local paths configured without committing them
- [ ] Build script proven to select the intended MSBuild
- [ ] Unit and integration test runners selected
- [ ] Fixed E2E scenario selected and licensed for local use
- [ ] UI Automation backend selected (`winapp` preferred; FlaUI fallback isolated behind script)
- [ ] A deliberately broken plugin causes E2E verification to fail
- [ ] A known-good smoke plugin causes E2E verification to pass

