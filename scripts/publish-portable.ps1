# Portable (no-MSIX) build entry point.
# Usage: powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/publish-portable.ps1
# Output: src/Desktop/BiliCopilot.UI/bin/Release/net9.0-windows10.0.22621.0/win-x64/portable/

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot 'src/Desktop/BiliCopilot.UI/BiliCopilot.UI.csproj'
$outDir = Join-Path $repoRoot 'src/Desktop/BiliCopilot.UI/bin/Release/net9.0-windows10.0.22621.0/win-x64/portable'

$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue).Source
$localDotnet = Join-Path $env:USERPROFILE '.dotnet-portable/dotnet.exe'
if (Test-Path -LiteralPath $localDotnet) {
    $dotnet = $localDotnet
}
if (-not $dotnet) { throw 'dotnet SDK not found' }

$sdk = (& $dotnet --version 2>$null)
if ([version]$sdk -lt [version]'9.0.100') { throw "Need .NET SDK 9.0+, current is $sdk" }

Write-Host "==> Publishing Portable x64 with SDK $sdk"
& $dotnet publish $project -c Release -r win-x64 --self-contained -p:Portable=true -p:PublishProfile=portable-win-x64 -p:SolutionDir="$repoRoot\"
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed, exit code $LASTEXITCODE" }

$required = @(
    'BiliCopilot.UI.exe',
    'Microsoft.ui.xaml.dll',
    'Assets/libmpv/x64/libmpv-2.dll',
    'Assets/ffmpeg/x64/ffmpeg.exe',
    'Assets/BBDown/x64/BBDown.exe',
    'Assets/BBDown/x64/bili.exe'
)
$missing = $required | Where-Object { -not (Test-Path -LiteralPath (Join-Path $outDir $_)) }
if ($missing) { throw "Missing files: $($missing -join ', ')" }
if (Test-Path -LiteralPath (Join-Path $outDir 'AppxManifest.xml')) { throw 'Output still has AppxManifest.xml; check WindowsPackageType' }

Write-Host "Done: $outDir"
Write-Host 'Run bili.exe login in Assets/BBDown/x64 before first download.'
