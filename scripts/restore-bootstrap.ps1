param([Parameter(Mandatory)] [string] $EvidenceDirectory)
. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
if (@(Get-Process -Name $config.bveProcessName -ErrorAction SilentlyContinue).Count -ne 0) {
    throw 'Close the configured BveTs process before restoration.'
}
$recordPath = Join-Path (Resolve-RepositoryPath $EvidenceDirectory) 'deployment.json'
Assert-ConfiguredFile $recordPath 'deployment record'
$record = Get-Content -LiteralPath $recordPath -Raw | ConvertFrom-Json
$destination = Join-Path $config.bveExExtensionDirectory 'CommEx.dll'
if ([IO.Path]::GetFullPath($record.destination) -ne [IO.Path]::GetFullPath($destination)) {
    throw 'Backup target does not match configured deployment target.'
}
if ((Get-FileHash -LiteralPath $destination).Hash -ne $record.sourceHash) {
    throw 'Installed DLL changed since deployment; automatic restoration refused.'
}
$backup = Join-Path (Split-Path $recordPath -Parent) 'previous-CommEx.dll'
Assert-ConfiguredFile $backup 'previous plugin backup'
if ((Get-FileHash -LiteralPath $backup).Hash -ne $record.previousHash) { throw 'Previous DLL backup hash mismatch.' }
Copy-Item -LiteralPath $backup -Destination $destination -Force
if ((Get-FileHash -LiteralPath $destination).Hash -ne $record.previousHash) { throw 'Restored DLL hash mismatch.' }
Write-Output 'PASS: previous plugin DLL restored; saved toggle states remain available for explicit recovery.'
