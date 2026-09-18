param(
    [int] $HostProcessId = 0,
    [switch] $CloseAfterTest,
    [switch] $ShowWindow
)
. (Join-Path $PSScriptRoot 'common.ps1')
$config = Get-LocalConfiguration
if (-not $config.e2eEnabled) { throw 'Required bootstrap host verification is disabled.' }
Assert-ConfiguredFile $config.bveExecutable 'bveExecutable'
Assert-ConfiguredDirectory $config.bveWorkingDirectory 'bveWorkingDirectory'
Assert-ConfiguredFile $config.bveExStatePath 'declared BveEX toggle-state path'
$expectedStatePath = Join-Path (Split-Path $config.bveExExtensionDirectory -Parent) 'LoadedExtensions.xml'
if ([IO.Path]::GetFullPath($config.bveExStatePath) -ne [IO.Path]::GetFullPath($expectedStatePath)) {
    throw 'bveExStatePath does not match the configured runtime state file.'
}
if ($HostProcessId -ne 0 -and -not $CloseAfterTest) { throw 'An attached bootstrap test requires -CloseAfterTest to verify shutdown.' }
$root = Get-RepositoryRoot
$plugin = Resolve-RepositoryPath $config.pluginOutputPath
$deployed = Join-Path $config.bveExExtensionDirectory 'CommEx.dll'
Assert-ConfiguredFile $deployed 'deployed bootstrap'
$expectedHash = (Get-FileHash -LiteralPath $plugin).Hash
if ((Get-FileHash -LiteralPath $deployed).Hash -ne $expectedHash) { throw 'Build and deployed DLL hashes differ.' }
& (Join-Path $PSScriptRoot 'test-bootstrap.ps1')
$winappPath = (Get-Command winapp -ErrorAction Stop).Source
Add-Type -Path (Join-Path $root 'tests\BveE2E\ProcessFileMappings.cs')
$evidence = New-EvidenceDirectory 'bootstrap-host'
$script:bootstrapUiStep = 0

function Invoke-BootstrapUi([string[]] $Arguments) {
    $script:bootstrapUiStep++
    $quoted = foreach ($argument in (@('ui') + $Arguments + @('--json'))) {
        $escaped = [regex]::Replace($argument, '(\\*)"', '$1$1\"')
        '"' + [regex]::Replace($escaped, '(\\+)$', '$1$1') + '"'
    }
    $startInfo = New-Object Diagnostics.ProcessStartInfo
    $startInfo.FileName = $winappPath
    $startInfo.Arguments = $quoted -join ' '
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $command = New-Object Diagnostics.Process
    $command.StartInfo = $startInfo
    try {
        [void]$command.Start()
        $stdout = $command.StandardOutput.ReadToEndAsync()
        $stderr = $command.StandardError.ReadToEndAsync()
        if (-not $command.WaitForExit(20000)) {
            $command.Kill()
            [void]$command.WaitForExit(5000)
            throw "UI command $($Arguments[0]) exceeded 20 seconds; its owned CLI process was stopped."
        }
        $raw = $stdout.Result
        $errorText = $stderr.Result
        $exitCode = $command.ExitCode
    } finally {
        $command.Dispose()
    }
    $raw | Set-Content -LiteralPath (Join-Path $evidence ("ui-{0:D2}.json" -f $script:bootstrapUiStep)) -Encoding UTF8
    if ($errorText) { $errorText | Set-Content -LiteralPath (Join-Path $evidence ("ui-{0:D2}-stderr.log" -f $script:bootstrapUiStep)) -Encoding UTF8 }
    if ($exitCode -ne 0) { throw "UI command $($Arguments[0]) failed with exit $exitCode; see raw evidence." }
    return ($raw | ConvertFrom-Json)
}

function Get-BootstrapElements($Nodes) {
    foreach ($node in $Nodes) {
        $node
        if ($node.PSObject.Properties['children']) { Get-BootstrapElements $node.children }
    }
}

$process = $null
$ownedLaunch = $false
$authorizedProcess = $false
$record = [ordered]@{
    scope = 'configured-host bootstrap only'
    startedAt = [DateTime]::UtcNow.ToString('o')
    commit = (& git -C $root rev-parse HEAD)
    status = 'running'
    dllSha256 = $expectedHash
    shutdownRequested = $false
    shutdownPassed = $false
}
$inputHashes = [ordered]@{}
foreach ($inputPath in @('scripts/test-bve-bootstrap.ps1', 'scripts/test-bootstrap.ps1', 'tests/Integration/bootstrap-contract.ps1', 'tests/BveE2E/assert-bootstrap-ui.ps1', 'tests/BveE2E/ProcessFileMappings.cs')) {
    $inputHashes[$inputPath] = (Get-FileHash -LiteralPath (Join-Path $root $inputPath)).Hash
}
$record.inputHashes = $inputHashes
try {
    if ($HostProcessId -eq 0) {
        if (@(Get-Process -Name $config.bveProcessName -ErrorAction SilentlyContinue).Count -ne 0) {
            throw 'An existing BveTs process is not owned by this invocation; supply its ID only if authorized.'
        }
        $style = if ($ShowWindow) { 'Normal' } else { 'Hidden' }
        $process = Start-Process -FilePath $config.bveExecutable -WorkingDirectory $config.bveWorkingDirectory -WindowStyle $style -PassThru
        $ownedLaunch = $true
    } else {
        $process = Get-Process -Id $HostProcessId -ErrorAction Stop
    }
    if ($process.Path -ne $config.bveExecutable) { throw 'Process executable differs from configured BveTs.' }
    $authorizedProcess = $true
    $record.processId = $process.Id
    $record.processStartUtc = $process.StartTime.ToUniversalTime().ToString('o')
    $record | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $evidence 'result.json') -Encoding UTF8
    $deadline = [DateTime]::UtcNow.AddSeconds(60)
    do {
        $windows = @(Invoke-BootstrapUi -Arguments @('list-windows', '-a', [string]$process.Id))
        $ready = $windows.Count -gt 0 -and (-not $ownedLaunch -or @($windows | Where-Object { $_.ownerHwnd -ne 0 }).Count -gt 0)
        if ($ready) { break }
        if ($process.HasExited) { throw 'BveTs exited before opening a window.' }
        Start-Sleep -Milliseconds 250
    } while ([DateTime]::UtcNow -lt $deadline)
    if (-not $ready) { throw 'BveTs startup dialog did not become ready in 60 seconds.' }
    $tree = Invoke-BootstrapUi -Arguments @('inspect', '-a', [string]$process.Id, '-d', '2')
    $scenarioElements = @(foreach ($window in $tree.windows) {
        Get-BootstrapElements $window.elements | Where-Object { $_.PSObject.Properties['automationId'] -and $_.automationId -eq 'ScenarioForm' }
    })
    if ($scenarioElements.Count -gt 0) {
        $windows = @(Invoke-BootstrapUi -Arguments @('list-windows', '-a', [string]$process.Id))
        $scenarioWindow = @($windows | Where-Object { $_.title -eq $scenarioElements[0].name })
        if ($scenarioWindow.Count -ne 1) { throw 'Scenario dialog handle is absent or ambiguous.' }
        Invoke-BootstrapUi -Arguments @('invoke', 'Close', '-w', [string]$scenarioWindow[0].hwnd) | Out-Null
    }
    $windows = @(Invoke-BootstrapUi -Arguments @('list-windows', '-a', [string]$process.Id))
    $versionWindow = @($windows | Where-Object { $_.title -like 'BveEX *' })
    if ($versionWindow.Count -eq 0) {
        $main = @($windows | Where-Object { $_.title -eq 'BVE Trainsim 6' })
        if ($main.Count -ne 1) { throw 'Expected one BVE 6 main window.' }
        $mainTree = Invoke-BootstrapUi -Arguments @('inspect', '-w', [string]$main[0].hwnd, '-d', '1')
        $mainElement = $mainTree.windows[0].elements[0]
        if (-not $mainElement.isEnabled) { throw 'Main window is disabled by a modal dialog; no mouse action is permitted.' }
        Invoke-BootstrapUi -Arguments @('click', $mainElement.selector, '-w', [string]$main[0].hwnd, '--right') | Out-Null
        $menuTree = Invoke-BootstrapUi -Arguments @('inspect', '-w', [string]$main[0].hwnd, '-d', '4')
        $menuItems = @(Get-BootstrapElements $menuTree.windows[0].elements | Where-Object { $_.type -eq 'MenuItem' -and $_.name -like 'BveEX *' })
        if ($menuItems.Count -ne 1) { throw 'Expected one BveEX plugin-list menu item.' }
        Invoke-BootstrapUi -Arguments @('invoke', $menuItems[0].selector, '-w', [string]$main[0].hwnd) | Out-Null
        $windows = @(Invoke-BootstrapUi -Arguments @('list-windows', '-a', [string]$process.Id))
        $versionWindow = @($windows | Where-Object { $_.title -like 'BveEX *' })
    }
    if ($versionWindow.Count -ne 1) { throw 'BveEX plugin-list dialog is absent or ambiguous.' }
    $dialogHandle = [string]$versionWindow[0].hwnd
    $snapshot = Invoke-BootstrapUi -Arguments @('inspect', '-w', $dialogHandle, '-d', '7')
    $snapshotPath = Join-Path $evidence 'plugin-list.json'
    $snapshot | ConvertTo-Json -Depth 50 | Set-Content -LiteralPath $snapshotPath -Encoding UTF8
    $row = & (Join-Path $root 'tests\BveE2E\assert-bootstrap-ui.ps1') -SnapshotPath $snapshotPath
    Invoke-BootstrapUi -Arguments @('scroll-into-view', $row.rowSelector, '-w', $dialogHandle) | Out-Null
    Invoke-BootstrapUi -Arguments @('screenshot', '-w', $dialogHandle, '-o', (Join-Path $evidence 'plugin-list.png')) | Out-Null
    if (-not [CommEx.Testing.ProcessFileMappings]::Contains($process.Id, $deployed)) { throw 'Expected DLL is not mapped by this BveTs process.' }
    if ((Get-FileHash -LiteralPath $deployed).Hash -ne $expectedHash) { throw 'Deployed DLL changed during host verification.' }
    $process.Refresh()
    if (-not $process.Responding) { throw 'BveTs is not responding after plugin inspection.' }
    $record.version = $row.version
    $record.description = $row.description
    $record.mappedDllPath = $deployed
    $record.responding = $true
    Invoke-BootstrapUi -Arguments @('invoke', 'OK', '-w', $dialogHandle) | Out-Null
    $record.status = 'passed'
} catch {
    $record.status = 'failed'
    $record.error = $_.Exception.Message
    throw
} finally {
    if ($authorizedProcess -and ($ownedLaunch -or $CloseAfterTest)) {
        if (-not $process.HasExited) {
            $process.Refresh()
            $requested = $process.CloseMainWindow()
            $record.shutdownRequested = $requested
            $record.shutdownPassed = $requested -and $process.WaitForExit(5000)
            if ($record.shutdownPassed) {
                $record.hostExitCode = $process.ExitCode
                $record.shutdownPassed = $process.ExitCode -eq 0
            }
        }
        if (-not $record.shutdownPassed) { $record.status = 'failed' }
    }
    $record.completedAt = [DateTime]::UtcNow.ToString('o')
    $record | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidence 'result.json') -Encoding UTF8
}
if ($record.status -ne 'passed') { throw "Bootstrap verification failed; see $evidence" }
Write-Output "PASS: live BveEX plugin identity, mapped DLL and responsiveness; evidence: $evidence"
