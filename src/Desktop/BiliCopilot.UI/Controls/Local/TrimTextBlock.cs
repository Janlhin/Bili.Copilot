// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 限制最大行数并显示省略号的文本控件.
/// </summary>
public sealed partial class TrimTextBlock : UserControl
{
    /// <summary>
    /// 标识 Text 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(TrimTextBlock), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 MaxLines 依赖属性.
    /// </summary>
    public static readonly DependencyProperty MaxLinesProperty = DependencyProperty.Register(
        "MaxLines", typeof(int), typeof(TrimTextBlock), new PropertyMetadata(0, OnMaxLinesChanged));

    /// <summary>
    /// 标识 IsTextSelectionEnabled 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsTextSelectionEnabledProperty = DependencyProperty.Register(
        "IsTextSelectionEnabled", typeof(bool), typeof(TrimTextBlock), new PropertyMetadata(false));

    /// <summary>
    /// 标识 TextAlignment 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
        "TextAlignment", typeof(TextAlignment), typeof(TrimTextBlock), new PropertyMetadata(TextAlignment.Left));

    /// <summary>
    /// 初始化 <see cref="TrimTextBlock"/> 的新实例.
    /// </summary>
    public TrimTextBlock()
        => InitializeComponent();

    /// <summary>
    /// 文本内容.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 最大显示行数.
    /// </summary>
    public int MaxLines
    {
        get => (int)GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    /// <summary>
    /// 是否允许选择文本.
    /// </summary>
    public bool IsTextSelectionEnabled
    {
        get => (bool)GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    /// <summary>
    /// 文本对齐方式.
    /// </summary>
    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    private static void OnMaxLinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is TrimTextBlock control)
        {
            control.Block.TextWrapping = control.MaxLines == 1 ? TextWrapping.NoWrap : TextWrapping.Wrap;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Block.TextWrapping = MaxLines == 1 ? TextWrapping.NoWrap : TextWrapping.Wrap;
}
