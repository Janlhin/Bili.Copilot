// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;

namespace BiliCopilot.UI.Forms;

/// <summary>
/// 无包标识（Portable）通道使用的精简启动窗口。
/// Richasy.WinUIKernel.Share 的 WindowBase/AppTitleBar 与 LayoutPageBase
/// 在 .NET 10 + WASDK 2.7 的未打包进程中会触发 WinRT stowed exception，
/// 因此这里改用原生 WinUI 窗口，避免依赖包标识的控件与资源加载路径。
/// </summary>
internal sealed class PortableStartupWindow : Window
{
    public PortableStartupWindow()
    {
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);

        var panel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 12,
        };

        panel.Children.Add(new TextBlock
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.AppName),
            FontSize = 24,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        panel.Children.Add(new TextBlock
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.AppDescription),
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        Content = panel;
    }
}
