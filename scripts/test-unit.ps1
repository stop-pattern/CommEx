. (Join-Path $PSScriptRoot 'common.ps1')
$root = Get-RepositoryRoot
$evidence = New-EvidenceDirectory 'unit'

$testAssemblies = @(Get-ChildItem -Path (Join-Path $root 'tests') -Recurse -Filter '*UnitTests.dll' `
    -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match '\\bin\\' })
if ($testAssemblies.Count -eq 0) {
    throw 'No unit-test assembly found. This is not a passing unit-test stage.'
}

$config = Get-LocalConfiguration
$vstest = $config.vstestPath
Assert-ConfiguredFile $vstest 'vstestPath'
$arguments = @($testAssemblies.FullName) + @('/Logger:trx', "/ResultsDirectory:$evidence")
Invoke-LoggedCommand -Executable $vstest -Arguments $arguments `
    -LogPath (Join-Path $evidence 'vstest.log') -WorkingDirectory $root

