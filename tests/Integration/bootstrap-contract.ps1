param(
    [Parameter(Mandatory)] [string] $PluginPath,
    [Parameter(Mandatory)] [string] $HostDirectory
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$hostAssembly = [Reflection.Assembly]::LoadFrom((Join-Path $HostDirectory 'BveEx.PluginHost.dll'))
$assembly = [Reflection.Assembly]::LoadFrom($PluginPath)
$entry = $assembly.GetType('CommEx.CommExMain', $false)
if ($null -eq $entry) { throw 'Missing required BveEX entry point: CommEx.CommExMain.' }
$baseType = $hostAssembly.GetType('BveEx.PluginHost.Plugins.AssemblyPluginBase', $true)
$extensionType = $hostAssembly.GetType('BveEx.PluginHost.Plugins.Extensions.IExtension', $true)
$builderType = $hostAssembly.GetType('BveEx.PluginHost.Plugins.PluginBuilder', $true)
if (-not $entry.IsSubclassOf($baseType) -or -not $extensionType.IsAssignableFrom($entry)) {
    throw 'Entry point does not implement the BveEX Extension contract.'
}
if ($null -eq $entry.GetConstructor([Type[]]@($builderType))) { throw 'Public PluginBuilder constructor is missing.' }
$attributes = @($entry.GetCustomAttributesData() | Where-Object { $_.AttributeType.FullName -eq 'BveEx.PluginHost.Plugins.PluginAttribute' })
$pluginType = $hostAssembly.GetType('BveEx.PluginHost.Plugins.PluginType', $true)
$extensionValue = [int][Enum]::Parse($pluginType, 'Extension')
if ($attributes.Count -ne 1 -or [int]$attributes[0].ConstructorArguments[0].Value -ne $extensionValue) {
    throw 'Plugin attribute must identify an Extension.'
}
$framework = @($assembly.GetCustomAttributesData() | Where-Object { $_.AttributeType.FullName -eq 'System.Runtime.Versioning.TargetFrameworkAttribute' })
if ($framework.Count -ne 1 -or $framework[0].ConstructorArguments[0].Value -ne '.NETFramework,Version=v4.8') {
    throw 'Plugin must target .NET Framework 4.8.'
}
foreach ($name in @('BveEx.PluginHost.dll', 'BveEx.dll', 'BveTypes.dll')) {
    if (Test-Path -LiteralPath (Join-Path (Split-Path $PluginPath -Parent) $name)) { throw "Host binary must not be copied to plugin output: $name" }
}
Write-Output 'PASS: net48 BveEX Extension entry contract; no redistributed host assemblies.'
