param(
    [Parameter(Mandatory)] [ValidatePattern('^[A-Fa-f0-9]{64}$')] [string] $ExpectedSha256
)
. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
$plugin = Resolve-RepositoryPath $config.pluginOutputPath
Assert-ConfiguredFile $plugin 'pluginOutputPath'
Assert-ConfiguredDirectory $config.bveExExtensionDirectory 'bveExExtensionDirectory'
if ((Split-Path $plugin -Leaf) -ne 'CommEx.dll') { throw 'Bootstrap deployment accepts only CommEx.dll.' }
if (@(Get-Process -Name $config.bveProcessName -ErrorAction SilentlyContinue).Count -ne 0) {
    throw 'Close the configured BveTs process before deploying; this script never stops an existing host.'
}
$sourceHash = (Get-FileHash -LiteralPath $plugin -Algorithm SHA256).Hash
if ($sourceHash -ne $ExpectedSha256) { throw 'Source DLL hash does not match the verified artifact.' }
& (Join-Path $PSScriptRoot 'test-bootstrap.ps1')
$statePath = $config.bveExStatePath
$expectedStatePath = Join-Path (Split-Path $config.bveExExtensionDirectory -Parent) 'LoadedExtensions.xml'
if ([IO.Path]::GetFullPath($statePath) -ne [IO.Path]::GetFullPath($expectedStatePath)) {
    throw 'bveExStatePath must identify the configured runtime toggle-state file.'
}
$destination = Join-Path $config.bveExExtensionDirectory 'CommEx.dll'
$evidence = New-EvidenceDirectory 'bootstrap-deploy'
$previousHash = $null
if (Test-Path -LiteralPath $destination -PathType Leaf) {
    $previousHash = (Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash
    Copy-Item -LiteralPath $destination -Destination (Join-Path $evidence 'previous-CommEx.dll')
    if ((Get-FileHash -LiteralPath (Join-Path $evidence 'previous-CommEx.dll')).Hash -ne $previousHash) {
        throw 'Previous DLL backup verification failed; deployment cancelled.'
    }
}
if (Test-Path -LiteralPath $statePath -PathType Leaf) {
    Copy-Item -LiteralPath $statePath -Destination (Join-Path $evidence 'previous-LoadedExtensions.xml')
}
$record = [ordered]@{
    startedAt = [DateTime]::UtcNow.ToString('o')
    commit = (& git -C (Get-RepositoryRoot) rev-parse HEAD)
    sourceHash = $sourceHash
    previousHash = $previousHash
    destination = $destination
    statePath = $statePath
    status = 'backed-up'
}
$record | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $evidence 'deployment.json') -Encoding UTF8
Copy-Item -LiteralPath $plugin -Destination $destination -Force
if ((Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash -ne $sourceHash) {
    throw "Deployed hash mismatch. Preserve recovery evidence at $evidence and restore before launching."
}
$record.status = 'deployed'
$record | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $evidence 'deployment.json') -Encoding UTF8
Write-Output "PASS: deployed verified bootstrap; backup and manifest: $evidence"
