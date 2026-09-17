param([switch] $SkipBveE2E)
$ErrorActionPreference = 'Stop'

$stages = @(
    'build.ps1',
    'test-unit.ps1',
    'test-integration.ps1'
)
if (-not $SkipBveE2E) { $stages += 'test-bve.ps1' }

foreach ($stage in $stages) {
    $path = Join-Path $PSScriptRoot $stage
    Write-Host "==> $stage"
    & $path
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

if ($SkipBveE2E) {
    Write-Warning 'BVE E2E was skipped. This result cannot complete a feature that requires E2E.'
}
Write-Host 'All selected verification stages passed.'

