// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Resolvers;
using Microsoft.UI.Xaml;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 短暂显示的状态提示浮层.
/// </summary>
public sealed partial class TipPopup : LayoutUserControlBase
{
    private const double DisplaySeconds = 3.5;

    /// <summary>
    /// 标识 Text 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(TipPopup), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 初始化 <see cref="TipPopup"/> 的新实例.
    /// </summary>
    public TipPopup()
        => InitializeComponent();

    /// <summary>
    /// 提示文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 显示提示并在一段时间后自动关闭.
    /// </summary>
    /// <param name="type">提示类型.</param>
    public async Task ShowAsync(InfoType type = InfoType.Information)
    {
        if (string.IsNullOrEmpty(Text))
        {
            Text = new PortableResourceToolkit().GetLocalizedString("Tip");
        }

        if (PopupContainer is not null)
        {
            PopupContainer.Status = type;
        }

        Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(DisplaySeconds));
        Visibility = Visibility.Collapsed;
    }
}
