// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using FluentSymbol = FluentIcons.Common.Symbol;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 可见性切换按钮的折叠方向.
/// </summary>
public enum VisibilityToggleButtonDirection
{
    /// <summary>
    /// 被控制的内容在左侧.
    /// </summary>
    LeftToRightVisible,

    /// <summary>
    /// 被控制的内容在右侧.
    /// </summary>
    RightToLeftVisible,
}

/// <summary>
/// 列宽分隔条.
/// </summary>
public sealed partial class ColumnSplitter : LayoutUserControlBase
{
    /// <summary>
    /// 标识 IsHide 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideProperty = DependencyProperty.Register(
        "IsHide", typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(false));

    /// <summary>
    /// 标识 ColumnWidth 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(
        "ColumnWidth", typeof(double), typeof(ColumnSplitter), new PropertyMetadata(200d));

    /// <summary>
    /// 标识 MinColumnWidth 依赖属性.
    /// </summary>
    public static readonly DependencyProperty MinColumnWidthProperty = DependencyProperty.Register(
        "MinColumnWidth", typeof(double), typeof(ColumnSplitter), new PropertyMetadata(0d));

    /// <summary>
    /// 标识 MaxColumnWidth 依赖属性.
    /// </summary>
    public static readonly DependencyProperty MaxColumnWidthProperty = DependencyProperty.Register(
        "MaxColumnWidth", typeof(double), typeof(ColumnSplitter), new PropertyMetadata(double.PositiveInfinity));

    /// <summary>
    /// 标识 IsHideButtonEnabled 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideButtonEnabledProperty = DependencyProperty.Register(
        "IsHideButtonEnabled", typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(true));

    /// <summary>
    /// 标识 IsInvertDirection 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsInvertDirectionProperty = DependencyProperty.Register(
        "IsInvertDirection", typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(false));

    /// <summary>
    /// 标识 HideTip 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HideTipProperty = DependencyProperty.Register(
        "HideTip", typeof(string), typeof(ColumnSplitter), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 ShowTip 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ShowTipProperty = DependencyProperty.Register(
        "ShowTip", typeof(string), typeof(ColumnSplitter), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 AlwaysShowButtonWhenCollapsed 依赖属性.
    /// </summary>
    public static readonly DependencyProperty AlwaysShowButtonWhenCollapsedProperty = DependencyProperty.Register(
        "AlwaysShowButtonWhenCollapsed", typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(false));

    /// <summary>
    /// 初始化 <see cref="ColumnSplitter"/> 的新实例.
    /// </summary>
    public ColumnSplitter()
        => InitializeComponent();

    /// <summary>
    /// 列是否被折叠.
    /// </summary>
    public bool IsHide
    {
        get => (bool)GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    /// <summary>
    /// 列的宽度.
    /// </summary>
    public double ColumnWidth
    {
        get => (double)GetValue(ColumnWidthProperty);
        set => SetValue(ColumnWidthProperty, value);
    }

    /// <summary>
    /// 列的最小宽度.
    /// </summary>
    public double MinColumnWidth
    {
        get => (double)GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    /// <summary>
    /// 列的最大宽度.
    /// </summary>
    public double MaxColumnWidth
    {
        get => (double)GetValue(MaxColumnWidthProperty);
        set => SetValue(MaxColumnWidthProperty, value);
    }

    /// <summary>
    /// 折叠按钮是否可用.
    /// </summary>
    public bool IsHideButtonEnabled
    {
        get => (bool)GetValue(IsHideButtonEnabledProperty);
        set => SetValue(IsHideButtonEnabledProperty, value);
    }

    /// <summary>
    /// 是否反转折叠方向.
    /// </summary>
    public bool IsInvertDirection
    {
        get => (bool)GetValue(IsInvertDirectionProperty);
        set => SetValue(IsInvertDirectionProperty, value);
    }

    /// <summary>
    /// 折叠提示文本.
    /// </summary>
    public string HideTip
    {
        get => (string)GetValue(HideTipProperty);
        set => SetValue(HideTipProperty, value);
    }

    /// <summary>
    /// 展开提示文本.
    /// </summary>
    public string ShowTip
    {
        get => (string)GetValue(ShowTipProperty);
        set => SetValue(ShowTipProperty, value);
    }

    /// <summary>
    /// 折叠后是否始终显示按钮.
    /// </summary>
    public bool AlwaysShowButtonWhenCollapsed
    {
        get => (bool)GetValue(AlwaysShowButtonWhenCollapsedProperty);
        set => SetValue(AlwaysShowButtonWhenCollapsedProperty, value);
    }
}

/// <summary>
/// 空状态占位控件.
/// </summary>
public sealed partial class EmptyHolder : LayoutUserControlBase
{
    /// <summary>
    /// 标识 Title 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title", typeof(string), typeof(EmptyHolder), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 Description 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        "Description", typeof(string), typeof(EmptyHolder), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 Emoji 依赖属性.
    /// </summary>
    public static readonly DependencyProperty EmojiProperty = DependencyProperty.Register(
        "Emoji", typeof(string), typeof(EmptyHolder), new PropertyMetadata("🥲"));

    /// <summary>
    /// 标识 ActionElement 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ActionElementProperty = DependencyProperty.Register(
        "ActionElement", typeof(object), typeof(EmptyHolder), new PropertyMetadata(null));

    /// <summary>
    /// 初始化 <see cref="EmptyHolder"/> 的新实例.
    /// </summary>
    public EmptyHolder()
        => InitializeComponent();

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 描述.
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// 表情符号.
    /// </summary>
    public string Emoji
    {
        get => (string)GetValue(EmojiProperty);
        set => SetValue(EmojiProperty, value);
    }

    /// <summary>
    /// 附加操作元素.
    /// </summary>
    public object? ActionElement
    {
        get => GetValue(ActionElementProperty);
        set => SetValue(ActionElementProperty, value);
    }
}

/// <summary>
/// 图标与文本组合控件.
/// </summary>
public sealed partial class IconTextBlock : LayoutUserControlBase
{
    /// <summary>
    /// 标识 Symbol 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        "Symbol", typeof(FluentSymbol), typeof(IconTextBlock), new PropertyMetadata(default(FluentSymbol)));

    /// <summary>
    /// 标识 Text 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(IconTextBlock), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 IconBrush 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconBrushProperty = DependencyProperty.Register(
        "IconBrush", typeof(Brush), typeof(IconTextBlock), new PropertyMetadata(null));

    /// <summary>
    /// 初始化 <see cref="IconTextBlock"/> 的新实例.
    /// </summary>
    public IconTextBlock()
        => InitializeComponent();

    /// <summary>
    /// 图标符号.
    /// </summary>
    public FluentSymbol Symbol
    {
        get => (FluentSymbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// 文本内容.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 图标画刷.
    /// </summary>
    public Brush? IconBrush
    {
        get => (Brush?)GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }
}

/// <summary>
/// 加载提示控件.
/// </summary>
public sealed partial class LoadingWidget : LayoutUserControlBase
{
    /// <summary>
    /// 标识 Text 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(LoadingWidget), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 初始化 <see cref="LoadingWidget"/> 的新实例.
    /// </summary>
    public LoadingWidget()
        => InitializeComponent();

    /// <summary>
    /// 提示文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}

/// <summary>
/// 微光加载占位控件.
/// </summary>
public sealed partial class Shimmer : LayoutUserControlBase
{
    /// <summary>
    /// 标识 Duration 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
        "Duration", typeof(object), typeof(Shimmer), new PropertyMetadata(TimeSpan.FromMilliseconds(1600)));

    /// <summary>
    /// 标识 IsActive 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        "IsActive", typeof(bool), typeof(Shimmer), new PropertyMetadata(false));

    /// <summary>
    /// 初始化 <see cref="Shimmer"/> 的新实例.
    /// </summary>
    public Shimmer()
        => InitializeComponent();

    /// <summary>
    /// 动画时长.
    /// </summary>
    public object Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// 是否处于加载状态.
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }
}

/// <summary>
/// 侧边展开按钮.
/// </summary>
public sealed partial class SideSplitButton : LayoutUserControlBase
{
    /// <summary>
    /// 标识 IsHide 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideProperty = DependencyProperty.Register(
        "IsHide", typeof(bool), typeof(SideSplitButton), new PropertyMetadata(false));

    /// <summary>
    /// 标识 IsInvertDirection 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsInvertDirectionProperty = DependencyProperty.Register(
        "IsInvertDirection", typeof(bool), typeof(SideSplitButton), new PropertyMetadata(false));

    /// <summary>
    /// 初始化 <see cref="SideSplitButton"/> 的新实例.
    /// </summary>
    public SideSplitButton()
        => InitializeComponent();

    /// <summary>
    /// 是否折叠.
    /// </summary>
    public bool IsHide
    {
        get => (bool)GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    /// <summary>
    /// 是否反转方向.
    /// </summary>
    public bool IsInvertDirection
    {
        get => (bool)GetValue(IsInvertDirectionProperty);
        set => SetValue(IsInvertDirectionProperty, value);
    }

    private void OnBtnClick(object sender, RoutedEventArgs e)
        => IsHide = !IsHide;
}

/// <summary>
/// 状态提示控件.
/// </summary>
public sealed partial class StatusTip : LayoutUserControlBase
{
    /// <summary>
    /// 标识 Text 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(StatusTip), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 Status 依赖属性.
    /// </summary>
    public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
        "Status", typeof(InfoType), typeof(StatusTip), new PropertyMetadata(InfoType.Information));

    /// <summary>
    /// 初始化 <see cref="StatusTip"/> 的新实例.
    /// </summary>
    public StatusTip()
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
    /// 提示类型.
    /// </summary>
    public InfoType Status
    {
        get => (InfoType)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
}

/// <summary>
/// 提示框头部.
/// </summary>
public sealed partial class TipHeader : LayoutUserControlBase
{
    /// <summary>
    /// 标识 Title 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title", typeof(string), typeof(TipHeader), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 Comment 依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommentProperty = DependencyProperty.Register(
        "Comment", typeof(string), typeof(TipHeader), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 初始化 <see cref="TipHeader"/> 的新实例.
    /// </summary>
    public TipHeader()
        => InitializeComponent();

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 补充说明.
    /// </summary>
    public string Comment
    {
        get => (string)GetValue(CommentProperty);
        set => SetValue(CommentProperty, value);
    }
}

/// <summary>
/// 可见性切换按钮.
/// </summary>
public sealed partial class VisibilityToggleButton : LayoutUserControlBase
{
    /// <summary>
    /// 标识 Direction 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
        "Direction", typeof(VisibilityToggleButtonDirection), typeof(VisibilityToggleButton), new PropertyMetadata(VisibilityToggleButtonDirection.LeftToRightVisible));

    /// <summary>
    /// 标识 IsHide 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideProperty = DependencyProperty.Register(
        "IsHide", typeof(bool), typeof(VisibilityToggleButton), new PropertyMetadata(false));

    /// <summary>
    /// 标识 AlwaysShowWhenCollapsed 依赖属性.
    /// </summary>
    public static readonly DependencyProperty AlwaysShowWhenCollapsedProperty = DependencyProperty.Register(
        "AlwaysShowWhenCollapsed", typeof(bool), typeof(VisibilityToggleButton), new PropertyMetadata(false));

    /// <summary>
    /// 标识 ShowTip 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ShowTipProperty = DependencyProperty.Register(
        "ShowTip", typeof(string), typeof(VisibilityToggleButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 标识 HideTip 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HideTipProperty = DependencyProperty.Register(
        "HideTip", typeof(string), typeof(VisibilityToggleButton), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 初始化 <see cref="VisibilityToggleButton"/> 的新实例.
    /// </summary>
    public VisibilityToggleButton()
        => InitializeComponent();

    /// <summary>
    /// 点击按钮时触发.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// 折叠方向.
    /// </summary>
    public VisibilityToggleButtonDirection Direction
    {
        get => (VisibilityToggleButtonDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    /// <summary>
    /// 是否折叠.
    /// </summary>
    public bool IsHide
    {
        get => (bool)GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    /// <summary>
    /// 折叠后是否始终显示按钮.
    /// </summary>
    public bool AlwaysShowWhenCollapsed
    {
        get => (bool)GetValue(AlwaysShowWhenCollapsedProperty);
        set => SetValue(AlwaysShowWhenCollapsedProperty, value);
    }

    /// <summary>
    /// 处于显示状态时的提示.
    /// </summary>
    public string ShowTip
    {
        get => (string)GetValue(ShowTipProperty);
        set => SetValue(ShowTipProperty, value);
    }

    /// <summary>
    /// 处于隐藏状态时的提示.
    /// </summary>
    public string HideTip
    {
        get => (string)GetValue(HideTipProperty);
        set => SetValue(HideTipProperty, value);
    }

    private void OnBtnClick(object sender, RoutedEventArgs e)
    {
        IsHide = !IsHide;
        Click?.Invoke(this, EventArgs.Empty);
    }
}
