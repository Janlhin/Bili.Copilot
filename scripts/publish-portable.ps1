# Portable (no-MSIX) build entry point.
# Usage: powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/publish-portable.ps1
# Output: src/Desktop/BiliCopilot.UI/bin/Release/net10.0-windows10.0.22621.0/win-x64/portable/

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot 'src/Desktop/BiliCopilot.UI/BiliCopilot.UI.csproj'
$outDir = Join-Path $repoRoot 'src/Desktop/BiliCopilot.UI/bin/Release/net10.0-windows10.0.22621.0/win-x64/portable'

$dotnet = (Get-Command dotnet -ErrorAction SilentlyContinue).Source
$localDotnet = Join-Path $env:USERPROFILE '.dotnet-portable/dotnet.exe'
if (Test-Path -LiteralPath $localDotnet) {
    $dotnet = $localDotnet
}
$dotnet10 = Join-Path $env:USERPROFILE '.dotnet10/dotnet.exe'
if (Test-Path -LiteralPath $dotnet10) {
    $dotnet = $dotnet10
}
if (-not $dotnet) { throw 'dotnet SDK not found' }

$sdk = (& $dotnet --version 2>$null)
if ([version]$sdk -lt [version]'10.0.100') { throw "Need .NET SDK 10.0+, current is $sdk" }

# Upstream does not ship native player/muxer binaries. Copy them from local paths
# or set LIBMPV_PATH / FFMPEG_PATH before running this script.
$assetsRoot = Join-Path $repoRoot 'src/Desktop/BiliCopilot.UI/Assets'
$libmpvSrc = if ($env:LIBMPV_PATH) { $env:LIBMPV_PATH } else { Join-Path $env:USERPROFILE 'Downloads/Compressed/mpv.net-DW v2024.01/libmpv-2.dll' }
$ffmpegSrc = if ($env:FFMPEG_PATH) { $env:FFMPEG_PATH } else { Join-Path $env:USERPROFILE 'AppData/Local/Programs/SmartSub/resources/app.asar.unpacked/node_modules/ffmpeg-static/ffmpeg.exe' }
$libmpvDstDir = Join-Path $assetsRoot 'libmpv/x64'
$ffmpegDstDir = Join-Path $assetsRoot 'ffmpeg/x64'
New-Item -ItemType Directory -Force -Path $libmpvDstDir, $ffmpegDstDir | Out-Null
if (Test-Path -LiteralPath $libmpvSrc) {
    Copy-Item -LiteralPath $libmpvSrc -Destination (Join-Path $libmpvDstDir 'libmpv-2.dll') -Force
} elseif (-not (Test-Path -LiteralPath (Join-Path $libmpvDstDir 'libmpv-2.dll'))) {
    Write-Warning 'libmpv-2.dll not found; playback will not work. Set LIBMPV_PATH.'
}
if (Test-Path -LiteralPath $ffmpegSrc) {
    Copy-Item -LiteralPath $ffmpegSrc -Destination (Join-Path $ffmpegDstDir 'ffmpeg.exe') -Force
} elseif (-not (Test-Path -LiteralPath (Join-Path $ffmpegDstDir 'ffmpeg.exe'))) {
    Write-Warning 'ffmpeg.exe not found; muxing will not work. Set FFMPEG_PATH.'
}

Write-Host "==> Publishing Portable x64 with SDK $sdk"
& $dotnet publish $project -c Release -r win-x64 --self-contained -p:Portable=true -p:PublishProfile=portable-win-x64 -p:SolutionDir="$repoRoot\src\"
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed, exit code $LASTEXITCODE" }

# WASDK XAML 二进制资源 (PRI/XBF) 不会写入无包标识的 portable 输出目录,
# 但 ms-appx:/// 资源解析依赖它们. 从中间构建目录按相对结构复制到输出目录.
$buildBinDir = Join-Path $repoRoot 'src/_build/Release/BiliCopilot.UI/bin'
if (-not (Test-Path -LiteralPath $buildBinDir)) {
    throw "Build output not found: $buildBinDir (required for PRI/XBF resources)"
}
$buildBinDir = (Resolve-Path -LiteralPath $buildBinDir).Path
Write-Host "==> Copying PRI/XBF resources from $buildBinDir"
$resourceCount = 0
Get-ChildItem -Path $buildBinDir -Recurse -File | Where-Object { $_.Extension -in '.pri', '.xbf' } | ForEach-Object {
    $relative = $_.FullName.Substring($buildBinDir.Length).TrimStart('\', '/')
    $destination = Join-Path $outDir $relative
    $destinationDir = Split-Path -Parent $destination
    if (-not (Test-Path -LiteralPath $destinationDir)) {
        New-Item -ItemType Directory -Force -Path $destinationDir | Out-Null
    }
    Copy-Item -LiteralPath $_.FullName -Destination $destination -Force
    $resourceCount++
}
Write-Host "    Copied $resourceCount PRI/XBF resource files."

$required = @(
    'BiliCopilot.UI.exe',
    'Microsoft.ui.xaml.dll',
    'BiliCopilot.UI.pri',
    'App.xbf',
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
