// Copyright (c) Bili Copilot. All rights reserved.

using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Markup;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 设置卡片的内容对齐方式.
/// </summary>
public enum ContentAlignment
{
    /// <summary>
    /// 右对齐.
    /// </summary>
    Right,

    /// <summary>
    /// 左对齐.
    /// </summary>
    Left,

    /// <summary>
    /// 纵向排列.
    /// </summary>
    Vertical,
}

/// <summary>
/// 可勾选的卡片按钮.
/// </summary>
public sealed class AppToggleCard : ToggleButton
{
    /// <summary>
    /// 标识 IsEnableCheck 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsEnableCheckProperty = DependencyProperty.Register(
        "IsEnableCheck", typeof(bool), typeof(AppToggleCard), new PropertyMetadata(true));

    /// <summary>
    /// 初始化 <see cref="AppToggleCard"/> 的新实例.
    /// </summary>
    public AppToggleCard()
        => DefaultStyleKey = typeof(AppToggleCard);

    /// <summary>
    /// 是否允许切换选中状态.
    /// </summary>
    public bool IsEnableCheck
    {
        get => (bool)GetValue(IsEnableCheckProperty);
        set => SetValue(IsEnableCheckProperty, value);
    }
}

/// <summary>
/// 通用卡片按钮.
/// </summary>
public sealed class CardControl : Button
{
    /// <summary>
    /// 初始化 <see cref="CardControl"/> 的新实例.
    /// </summary>
    public CardControl()
        => DefaultStyleKey = typeof(CardControl);
}

/// <summary>
/// 可拖拽的卡片控件.
/// </summary>
[ContentProperty(Name = "Content")]
public sealed class DraggableCardControl : LayoutControlBase
{
    /// <summary>
    /// 标识 IsPointerOver 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsPointerOverProperty = DependencyProperty.Register(
        "IsPointerOver", typeof(bool), typeof(DraggableCardControl), new PropertyMetadata(false));

    /// <summary>
    /// 标识 IsPressed 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register(
        "IsPressed", typeof(bool), typeof(DraggableCardControl), new PropertyMetadata(false));

    /// <summary>
    /// 标识 Command 依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        "Command", typeof(ICommand), typeof(DraggableCardControl), new PropertyMetadata(null));

    /// <summary>
    /// 标识 Content 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        "Content", typeof(object), typeof(DraggableCardControl), new PropertyMetadata(null));

    /// <summary>
    /// 初始化 <see cref="DraggableCardControl"/> 的新实例.
    /// </summary>
    public DraggableCardControl()
        => DefaultStyleKey = typeof(DraggableCardControl);

    /// <summary>
    /// 指针是否悬停在控件上.
    /// </summary>
    public bool IsPointerOver
    {
        get => (bool)GetValue(IsPointerOverProperty);
        set => SetValue(IsPointerOverProperty, value);
    }

    /// <summary>
    /// 控件是否处于按下状态.
    /// </summary>
    public bool IsPressed
    {
        get => (bool)GetValue(IsPressedProperty);
        set => SetValue(IsPressedProperty, value);
    }

    /// <summary>
    /// 点击时执行的命令.
    /// </summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// 卡片内容.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
}

/// <summary>
/// 设置页卡片.
/// </summary>
public sealed class SettingsCard : ButtonBase
{
    /// <summary>
    /// 标识 Header 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        "Header", typeof(object), typeof(SettingsCard), new PropertyMetadata(null));

    /// <summary>
    /// 标识 Description 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        "Description", typeof(object), typeof(SettingsCard), new PropertyMetadata(null));

    /// <summary>
    /// 标识 HeaderIcon 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
        "HeaderIcon", typeof(IconElement), typeof(SettingsCard), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ActionIcon 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ActionIconProperty = DependencyProperty.Register(
        "ActionIcon", typeof(IconElement), typeof(SettingsCard), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ActionIconToolTip 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ActionIconToolTipProperty = DependencyProperty.Register(
        "ActionIconToolTip", typeof(string), typeof(SettingsCard), new PropertyMetadata(null));

    /// <summary>
    /// 标识 IsClickEnabled 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsClickEnabledProperty = DependencyProperty.Register(
        "IsClickEnabled", typeof(bool), typeof(SettingsCard), new PropertyMetadata(false));

    /// <summary>
    /// 标识 ContentAlignment 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentAlignmentProperty = DependencyProperty.Register(
        "ContentAlignment", typeof(ContentAlignment), typeof(SettingsCard), new PropertyMetadata(ContentAlignment.Right));

    /// <summary>
    /// 标识 IsActionIconVisible 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsActionIconVisibleProperty = DependencyProperty.Register(
        "IsActionIconVisible", typeof(bool), typeof(SettingsCard), new PropertyMetadata(true));

    /// <summary>
    /// 初始化 <see cref="SettingsCard"/> 的新实例.
    /// </summary>
    public SettingsCard()
        => DefaultStyleKey = typeof(SettingsCard);

    /// <summary>
    /// 标题.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// 描述.
    /// </summary>
    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// 标题图标.
    /// </summary>
    public IconElement? HeaderIcon
    {
        get => (IconElement?)GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    /// <summary>
    /// 操作图标.
    /// </summary>
    public IconElement? ActionIcon
    {
        get => (IconElement?)GetValue(ActionIconProperty);
        set => SetValue(ActionIconProperty, value);
    }

    /// <summary>
    /// 操作图标的提示文本.
    /// </summary>
    public string? ActionIconToolTip
    {
        get => (string?)GetValue(ActionIconToolTipProperty);
        set => SetValue(ActionIconToolTipProperty, value);
    }

    /// <summary>
    /// 是否可点击.
    /// </summary>
    public bool IsClickEnabled
    {
        get => (bool)GetValue(IsClickEnabledProperty);
        set => SetValue(IsClickEnabledProperty, value);
    }

    /// <summary>
    /// 内容对齐方式.
    /// </summary>
    public ContentAlignment ContentAlignment
    {
        get => (ContentAlignment)GetValue(ContentAlignmentProperty);
        set => SetValue(ContentAlignmentProperty, value);
    }

    /// <summary>
    /// 是否显示操作图标.
    /// </summary>
    public bool IsActionIconVisible
    {
        get => (bool)GetValue(IsActionIconVisibleProperty);
        set => SetValue(IsActionIconVisibleProperty, value);
    }
}

/// <summary>
/// 可展开的设置项集合.
/// </summary>
[ContentProperty(Name = "Content")]
public sealed class SettingsExpander : Control
{
    /// <summary>
    /// 标识 Items 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        "Items", typeof(IList<object>), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ItemsSource 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        "ItemsSource", typeof(object), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ItemTemplate 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        "ItemTemplate", typeof(object), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ItemContainerStyleSelector 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemContainerStyleSelectorProperty = DependencyProperty.Register(
        "ItemContainerStyleSelector", typeof(StyleSelector), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 Header 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        "Header", typeof(object), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 Description 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        "Description", typeof(object), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 HeaderIcon 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
        "HeaderIcon", typeof(IconElement), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 Content 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        "Content", typeof(object), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ItemsHeader 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsHeaderProperty = DependencyProperty.Register(
        "ItemsHeader", typeof(UIElement), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ItemsFooter 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsFooterProperty = DependencyProperty.Register(
        "ItemsFooter", typeof(UIElement), typeof(SettingsExpander), new PropertyMetadata(null));

    /// <summary>
    /// 标识 IsExpanded 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        "IsExpanded", typeof(bool), typeof(SettingsExpander), new PropertyMetadata(false));

    /// <summary>
    /// 初始化 <see cref="SettingsExpander"/> 的新实例.
    /// </summary>
    public SettingsExpander()
        => DefaultStyleKey = typeof(SettingsExpander);

    /// <summary>
    /// 设置项集合.
    /// </summary>
    public IList<object>? Items
    {
        get => (IList<object>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// 设置项数据源.
    /// </summary>
    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// 设置项模板.
    /// </summary>
    public object? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// 设置项容器样式选择器.
    /// </summary>
    public StyleSelector? ItemContainerStyleSelector
    {
        get => (StyleSelector?)GetValue(ItemContainerStyleSelectorProperty);
        set => SetValue(ItemContainerStyleSelectorProperty, value);
    }

    /// <summary>
    /// 标题.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// 描述.
    /// </summary>
    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// 标题图标.
    /// </summary>
    public IconElement? HeaderIcon
    {
        get => (IconElement?)GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    /// <summary>
    /// 展开区域内容.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// 设置项列表头部.
    /// </summary>
    public UIElement? ItemsHeader
    {
        get => (UIElement?)GetValue(ItemsHeaderProperty);
        set => SetValue(ItemsHeaderProperty, value);
    }

    /// <summary>
    /// 设置项列表尾部.
    /// </summary>
    public UIElement? ItemsFooter
    {
        get => (UIElement?)GetValue(ItemsFooterProperty);
        set => SetValue(ItemsFooterProperty, value);
    }

    /// <summary>
    /// 是否展开.
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
}
