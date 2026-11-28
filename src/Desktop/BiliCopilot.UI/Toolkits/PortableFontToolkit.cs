// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// Portable 免安装通道的字体列表工具.
/// Richasy 的 SharedFontToolkit 使用 Microsoft.Graphics.Canvas.Text WinRT API,
/// 在无包标识进程中会触发 0x80073D54 stowed exception; 这里返回常用中英文字体.
/// </summary>
internal sealed class PortableFontToolkit : IFontToolkit
{
    private static readonly string[] FontNames =
    [
        "Microsoft YaHei UI",
        "Microsoft YaHei",
        "SimHei",
        "SimSun",
        "KaiTi",
        "Segoe UI",
        "Arial",
        "Consolas",
    ];

    public Task<IReadOnlyList<SystemFont>> GetFontsAsync(bool useWin2D = true)
        => Task.FromResult<IReadOnlyList<SystemFont>>(FontNames.Select(p => new SystemFont { LocalName = p, EngName = p }).ToList());
}
