param([Parameter(Mandatory)] [string] $SnapshotPath)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$tree = Get-Content -LiteralPath $SnapshotPath -Raw | ConvertFrom-Json
$dialog = @($tree.windows | Where-Object { $_.title -like 'BveEX *' })
if ($dialog.Count -ne 1) { throw 'Expected exactly one BveEX plugin-list dialog.' }
function Find-PluginRows($Nodes) {
    foreach ($node in $Nodes) {
        if ($node.type -eq 'ListItem' -and $node.PSObject.Properties['name'] -and $node.name -eq 'CommEx.dll') { $node }
        if ($node.PSObject.Properties['children']) { Find-PluginRows $node.children }
    }
}
$rows = @(Find-PluginRows $dialog[0].elements)
if ($rows.Count -ne 1) { throw 'Expected exactly one instantiated CommEx plugin row.' }
$expected = @('CommEx.dll', 'CommEx', '1.0.0.0', '-', 'CommEx development bootstrap (no communication features)')
for ($index = 0; $index -lt $expected.Count; $index++) {
    $cell = @($rows[0].children | Where-Object { $_.automationId -eq "ListViewSubItem-$index" })
    if ($cell.Count -ne 1 -or $cell[0].name -ne $expected[$index]) { throw "Unexpected CommEx plugin-list column $index." }
}
[pscustomobject]@{ dialogHwnd=$dialog[0].hwnd; rowSelector=$rows[0].selector; version=$expected[2]; description=$expected[4] }
