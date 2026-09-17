. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
$root = Get-RepositoryRoot
$codex = Get-Command codex -ErrorAction SilentlyContinue
if (-not $codex) { throw 'codex was not found on PATH.' }

$taskFile = Join-Path $root 'specs\000-product\tasks.md'
$pending = @(Select-String -LiteralPath $taskFile -Pattern '^- \[ \] (F\d{3}) (.+)$')
if ($pending.Count -eq 0) {
    Write-Host 'No pending feature was found.'
    exit 0
}

foreach ($match in $pending) {
    $featureId = $match.Matches[0].Groups[1].Value
    $featureTitle = $match.Matches[0].Groups[2].Value
    $attemptPassed = $false

    for ($attempt = 1; $attempt -le $config.maximumFeatureAttempts; $attempt++) {
        $prompt = @"
Work on exactly $featureId ($featureTitle). Follow AGENTS.md and the approved specification.
Inspect existing evidence before changing code. Run the repository verification command.
Do not mark the feature complete or commit unless all required levels pass. If blocked, write a
report that states evidence, falsified hypotheses, and the smallest required human action.
This is autonomous attempt $attempt of $($config.maximumFeatureAttempts).
"@
        & $codex.Source exec --full-auto $prompt
        if ($LASTEXITCODE -ne 0) { continue }

        & (Join-Path $PSScriptRoot 'verify.ps1')
        if ($LASTEXITCODE -eq 0) {
            $attemptPassed = $true
            if ($config.commitPassingFeatures) {
                & git -C $root add --all
                & git -C $root commit -m "feat($featureId): complete verified feature"
                if ($LASTEXITCODE -ne 0) { throw "Verification passed but commit failed for $featureId." }
            }
            break
        }
    }

    if (-not $attemptPassed) {
        Write-Warning "$featureId reached its attempt limit and is BLOCKED."
    }
}

