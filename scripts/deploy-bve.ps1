. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
$plugin = Resolve-RepositoryPath $config.pluginOutputPath
Assert-ConfiguredFile $plugin 'pluginOutputPath'
Assert-ConfiguredDirectory $config.bveExExtensionDirectory 'bveExExtensionDirectory'

$destination = Join-Path $config.bveExExtensionDirectory (Split-Path $plugin -Leaf)
Copy-Item -LiteralPath $plugin -Destination $destination -Force
Write-Output $destination

