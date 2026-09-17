param([switch] $Clean)
. (Join-Path $PSScriptRoot 'common.ps1')

$config = Get-LocalConfiguration
$root = Get-RepositoryRoot
$solution = Resolve-RepositoryPath $config.solutionPath
Assert-ConfiguredFile $solution 'solutionPath'

$msbuild = $config.msbuildPath
if ([string]::IsNullOrWhiteSpace($msbuild)) {
    $vswhere = [Environment]::ExpandEnvironmentVariables($config.vswherePath)
    Assert-ConfiguredFile $vswhere 'vswherePath'
    $msbuild = (& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild `
        -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1)
}
Assert-ConfiguredFile $msbuild 'MSBuild'

$evidence = New-EvidenceDirectory 'build'
$target = if ($Clean) { 'Rebuild' } else { 'Build' }
$arguments = @(
    $solution,
    "/t:$target",
    "/p:Configuration=$($config.configuration)",
    "/p:Platform=$($config.platform)",
    '/m',
    '/nologo',
    '/verbosity:minimal'
)
Invoke-LoggedCommand -Executable $msbuild -Arguments $arguments `
    -LogPath (Join-Path $evidence 'msbuild.log') -WorkingDirectory $root

