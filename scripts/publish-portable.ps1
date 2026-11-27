# 免安装 (Portable) 构建入口.
# 用法: pwsh -NoLogo -NoProfile -File ./scripts/publish-portable.ps1
# 产物: src/Desktop/BiliCopilot.UI/bin/Release/net9.0-windows10.0.22621.0/win-x64/portable/

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot 'src/Desktop/BiliCopilot.UI/BiliCopilot.UI.csproj'
$outDir = Join-Path $repoRoot 'src/Desktop/BiliCopilot.UI/bin/Release/net9.0-windows10.0.22621.0/win-x64/portable'

$sdk = (& dotnet --version 2>$null)
if (-not $sdk) { throw '未找到 dotnet SDK' }
if ([version]$sdk -lt [version]'9.0.100') { throw "需要 .NET SDK 9.0+, 当前为 $sdk" }

Write-Host "==> 发布 Portable x64 ($sdk)"
& dotnet publish $project -c Release -r win-x64 --self-contained -p:Portable=true -p:PublishProfile=portable-win-x64
if ($LASTEXITCODE -ne 0) { throw "dotnet publish 失败, 退出码 $LASTEXITCODE" }

$required = @(
    'BiliCopilot.UI.exe',
    'Microsoft.ui.xaml.dll',
    'Assets/libmpv/x64/libmpv-2.dll',
    'Assets/ffmpeg/x64/ffmpeg.exe',
    'Assets/BBDown/x64/BBDown.exe',
    'Assets/BBDown/x64/bili.exe'
)
$missing = $required | Where-Object { -not (Test-Path -LiteralPath (Join-Path $outDir $_)) }
if ($missing) { throw "缺少关键文件: $($missing -join ', ')" }
if (Test-Path -LiteralPath (Join-Path $outDir 'AppxManifest.xml')) { throw '输出目录仍包含 AppxManifest.xml, 请检查 WindowsPackageType' }

Write-Host "完成: $outDir"
Write-Host '首次使用请在 Assets/BBDown/x64 目录执行 bili.exe login 完成下载器登录。'
