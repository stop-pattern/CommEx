. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
if (-not $config.e2eEnabled) {
    throw 'BVE E2E is disabled. A disabled required stage is not PASS.'
}

Assert-ConfiguredFile $config.bveExecutable 'bveExecutable'
Assert-ConfiguredDirectory $config.bveWorkingDirectory 'bveWorkingDirectory'
Assert-ConfiguredFile $config.e2eScenarioPath 'e2eScenarioPath'

$backend = $config.uiAutomationBackend
if ($backend -ne 'winapp' -and $backend -ne 'flaui') {
    throw "Unsupported UI automation backend: $backend"
}

# The selectors and exact scenario-loading operations depend on the approved BVE/BveEX versions.
# This deliberate hard failure prevents a launch-only smoke test from being reported as E2E success.
throw @'
BVE E2E selectors are not implemented yet. Complete F001 by specifying stable selectors and checks for:
1. BVE window readiness and fixed scenario load
2. BveEX Extension load confirmation
3. settings UI invoke/set-value/apply
4. TestPeer packet assertion
5. responsiveness, log, screenshot, and clean shutdown evidence
'@

