. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
& (Join-Path (Get-RepositoryRoot) 'tests\Integration\bootstrap-contract.ps1') `
    -PluginPath (Resolve-RepositoryPath $config.pluginOutputPath) `
    -HostDirectory (Split-Path $config.bveExExtensionDirectory -Parent)
