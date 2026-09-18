. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
foreach ($name in @('msbuildPath', 'vstestPath', 'bveExecutable')) {
    Assert-ConfiguredFile $config.$name $name
}
Assert-ConfiguredDirectory $config.bveExExtensionDirectory 'bveExExtensionDirectory'
$runtimeDirectory = Split-Path $config.bveExExtensionDirectory -Parent
$reference = Join-Path $runtimeDirectory 'BveEx.PluginHost.dll'
Assert-ConfiguredFile $reference 'BveEX PluginHost reference'
$identity = [Reflection.AssemblyName]::GetAssemblyName($reference)
if ($identity.Name -ne 'BveEx.PluginHost' -or $identity.Version.Major -ne 2) {
    throw 'The bootstrap requires the configured BveEX 2 PluginHost installation.'
}
$propsPath = Join-Path (Get-RepositoryRoot) 'src\CommEx\CommEx.local.props'
$escapedDirectory = [Security.SecurityElement]::Escape($runtimeDirectory)
$content = "<Project><PropertyGroup><BveExRuntimeDirectory>$escapedDirectory</BveExRuntimeDirectory></PropertyGroup></Project>"
[IO.File]::WriteAllText($propsPath, $content + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
Write-Output "Development reference configured: $($identity.Name) $($identity.Version). Host binaries remain external."
