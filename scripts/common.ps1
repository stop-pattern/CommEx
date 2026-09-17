Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RepositoryRoot {
    return (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
}

function Get-LocalConfiguration {
    $root = Get-RepositoryRoot
    $path = Join-Path $root 'config\repo.local.json'
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Missing $path. Copy config/repo.local.example.json and set machine-local paths."
    }

    $config = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
    return $config
}

function Resolve-RepositoryPath([string] $Path) {
    if ([System.IO.Path]::IsPathRooted($Path)) { return $Path }
    return Join-Path (Get-RepositoryRoot) $Path
}

function Assert-ConfiguredFile([string] $Path, [string] $Name) {
    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "$Name is not configured as an existing file: $Path"
    }
}

function Assert-ConfiguredDirectory([string] $Path, [string] $Name) {
    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path -PathType Container)) {
        throw "$Name is not configured as an existing directory: $Path"
    }
}

function New-EvidenceDirectory([string] $Stage) {
    $root = Get-RepositoryRoot
    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $path = Join-Path $root "artifacts\$stamp-$Stage"
    New-Item -ItemType Directory -Path $path -Force | Out-Null
    return $path
}

function Invoke-LoggedCommand {
    param(
        [Parameter(Mandatory)] [string] $Executable,
        [Parameter(Mandatory)] [string[]] $Arguments,
        [Parameter(Mandatory)] [string] $LogPath,
        [string] $WorkingDirectory = (Get-RepositoryRoot)
    )

    & $Executable @Arguments 2>&1 | Tee-Object -FilePath $LogPath
    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) {
        throw "Command failed with exit code $exitCode. See $LogPath"
    }
}

