. (Join-Path $PSScriptRoot 'common.ps1')
$root = Get-RepositoryRoot
$evidence = New-EvidenceDirectory 'unit'

$testAssemblies = @(Get-ChildItem -Path (Join-Path $root 'tests') -Recurse -Filter '*UnitTests.dll' `
    -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match '\\bin\\' })
if ($testAssemblies.Count -eq 0) {
    throw 'No unit-test assembly found. This is not a passing unit-test stage.'
}

$vstest = Get-Command vstest.console.exe -ErrorAction SilentlyContinue
if (-not $vstest) { throw 'vstest.console.exe was not found on PATH.' }
$arguments = @($testAssemblies.FullName) + @('/Logger:trx', "/ResultsDirectory:$evidence")
Invoke-LoggedCommand -Executable $vstest.Source -Arguments $arguments `
    -LogPath (Join-Path $evidence 'vstest.log') -WorkingDirectory $root

