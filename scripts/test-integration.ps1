. (Join-Path $PSScriptRoot 'common.ps1')
$root = Get-RepositoryRoot
$evidence = New-EvidenceDirectory 'integration'

$testAssemblies = @(Get-ChildItem -Path (Join-Path $root 'tests') -Recurse -Filter '*IntegrationTests.dll' `
    -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match '\\bin\\' })
if ($testAssemblies.Count -eq 0) {
    throw 'No integration-test assembly found. This is not a passing integration-test stage.'
}

$config = Get-LocalConfiguration
$vstest = $config.vstestPath
Assert-ConfiguredFile $vstest 'vstestPath'
$arguments = @($testAssemblies.FullName) + @('/Logger:trx', "/ResultsDirectory:$evidence")
Invoke-LoggedCommand -Executable $vstest -Arguments $arguments `
    -LogPath (Join-Path $evidence 'vstest.log') -WorkingDirectory $root

