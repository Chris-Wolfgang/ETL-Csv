#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Compares the current build's public API against the latest published
    Wolfgang.Etl.Csv NuGet package to detect breaking API changes.

.DESCRIPTION
    Downloads the most recent stable Wolfgang.Etl.Csv .nupkg from NuGet.org,
    builds the local src project, then runs Microsoft.DotNet.ApiCompat.Tool
    against both. Any breaking change (removed type, removed member,
    signature change, etc.) is reported with a non-zero exit code.

    Designed to run pre-release as a guard against accidental API breakage.
    SemVer policy: PATCH/MINOR releases must NOT break ABI; MAJOR releases
    may, but the diff should still be reviewed and called out in the
    migration guide (docs/migration/).

.PARAMETER BaselineVersion
    The published Wolfgang.Etl.Csv version to diff against. Defaults to
    "latest" which resolves to the most recent stable version on NuGet.org.

.PARAMETER ProjectPath
    Path to the src csproj. Defaults to src/Wolfgang.Etl.Csv/Wolfgang.Etl.Csv.csproj.

.EXAMPLE
    pwsh ./scripts/check-api-compat.ps1
    pwsh ./scripts/check-api-compat.ps1 -BaselineVersion 0.1.0

.NOTES
    Requires Microsoft.DotNet.ApiCompat.Tool. Installed on first run if absent.

    This is intentionally a script-on-demand rather than a CI step. Running
    on every PR catches every accidental change but creates noise on PRs
    that deliberately add API surface (which is most non-trivial PRs in a
    library's early life). Pre-release run is the right point: it answers
    "am I about to ship a breaking change in a release that shouldn't?".

    When the library reaches a 1.x stability promise, consider moving this
    to release.yaml as a release gate.
#>

[CmdletBinding()]
param
(
    [string]$BaselineVersion = 'latest',
    [string]$ProjectPath = 'src/Wolfgang.Etl.Csv/Wolfgang.Etl.Csv.csproj',
    [string]$TargetFramework = 'net10.0'
)

$ErrorActionPreference = 'Stop'

# Ensure the ApiCompat tool is available.
$toolList = dotnet tool list -g 2>&1 | Out-String
if ($toolList -notmatch 'microsoft\.dotnet\.apicompat\.tool')
{
    Write-Host 'Installing Microsoft.DotNet.ApiCompat.Tool...'
    dotnet tool install -g Microsoft.DotNet.ApiCompat.Tool
}

# Resolve the baseline version.
if ($BaselineVersion -eq 'latest')
{
    Write-Host 'Resolving latest stable Wolfgang.Etl.Csv from NuGet.org...'
    $catalog = Invoke-RestMethod -Uri 'https://api.nuget.org/v3-flatcontainer/wolfgang.etl.csv/index.json'
    $stable = $catalog.versions | Where-Object { $_ -notmatch '-' } | Select-Object -Last 1
    if (-not $stable)
    {
        throw 'No stable Wolfgang.Etl.Csv versions found on NuGet.org. Pass -BaselineVersion explicitly.'
    }
    $BaselineVersion = $stable
}

Write-Host "Baseline: Wolfgang.Etl.Csv $BaselineVersion"

# Stage workspace.
$workDir = New-Item -ItemType Directory -Force -Path './artifacts/api-compat'
$baselineNupkg = Join-Path $workDir.FullName "Wolfgang.Etl.Csv.$BaselineVersion.nupkg"
$baselineExtract = Join-Path $workDir.FullName "baseline-$BaselineVersion"

# Download baseline nupkg.
if (-not (Test-Path $baselineNupkg))
{
    Write-Host "Downloading $BaselineVersion .nupkg..."
    Invoke-WebRequest `
        -Uri "https://api.nuget.org/v3-flatcontainer/wolfgang.etl.csv/$BaselineVersion/wolfgang.etl.csv.$BaselineVersion.nupkg" `
        -OutFile $baselineNupkg
}

if (Test-Path $baselineExtract) { Remove-Item -Recurse -Force $baselineExtract }
Expand-Archive -Path $baselineNupkg -DestinationPath $baselineExtract

$baselineDll = Get-ChildItem -Path $baselineExtract -Recurse -Filter 'Wolfgang.Etl.Csv.dll' |
    Where-Object { $_.Directory.Name -eq $TargetFramework } |
    Select-Object -First 1

if (-not $baselineDll)
{
    throw "Baseline .nupkg has no Wolfgang.Etl.Csv.dll under lib/$TargetFramework/. Try a different -TargetFramework."
}

# Build current.
Write-Host "Building current ($ProjectPath, $TargetFramework)..."
dotnet build $ProjectPath -c Release -f $TargetFramework | Out-Host
$currentDir = Split-Path $ProjectPath -Parent
$currentDll = Join-Path $currentDir "bin/Release/$TargetFramework/Wolfgang.Etl.Csv.dll"

if (-not (Test-Path $currentDll))
{
    throw "Current build .dll not found at $currentDll."
}

# Run ApiCompat.
Write-Host ''
Write-Host '== Running ApiCompat =='
Write-Host "Baseline: $($baselineDll.FullName)"
Write-Host "Current:  $currentDll"
Write-Host ''

apicompat --left $($baselineDll.FullName) --right $currentDll
$exit = $LASTEXITCODE

Write-Host ''
if ($exit -eq 0)
{
    Write-Host "✅ No breaking API changes vs $BaselineVersion."
}
else
{
    Write-Host "❌ Breaking API changes detected vs $BaselineVersion."
    Write-Host '   Review the report above. SemVer policy says PATCH/MINOR releases must not break ABI.'
    Write-Host '   If this break is intentional (MAJOR release), document it in docs/migration/.'
}

exit $exit
