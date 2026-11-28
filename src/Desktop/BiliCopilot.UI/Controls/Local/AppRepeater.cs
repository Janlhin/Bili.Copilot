// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 支持自定义容器模板的列表控件.
/// </summary>
public sealed partial class AppRepeater : UserControl
{
    /// <summary>
    /// 标识 ItemsSource 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        "ItemsSource", typeof(object), typeof(AppRepeater), new PropertyMetadata(null, OnItemsSourceChanged));

    /// <summary>
    /// 标识 RepeaterTabFocusNavigation 依赖属性.
    /// </summary>
    public static readonly DependencyProperty RepeaterTabFocusNavigationProperty = DependencyProperty.Register(
        "RepeaterTabFocusNavigation", typeof(KeyboardNavigationMode), typeof(AppRepeater), new PropertyMetadata((KeyboardNavigationMode)2));

    /// <summary>
    /// 标识 ItemTemplateSelector 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(
        "ItemTemplateSelector", typeof(DataTemplateSelector), typeof(AppRepeater), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ItemTemplate 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        "ItemTemplate", typeof(DataTemplate), typeof(AppRepeater), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ItemContainerTemplate 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemContainerTemplateProperty = DependencyProperty.Register(
        "ItemContainerTemplate", typeof(DataTemplate), typeof(AppRepeater), new PropertyMetadata(null));

    /// <summary>
    /// 标识 Layout 依赖属性.
    /// </summary>
    public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
        "Layout", typeof(VirtualizingLayout), typeof(AppRepeater), new PropertyMetadata(null));

    /// <summary>
    /// 初始化 <see cref="AppRepeater"/> 的新实例.
    /// </summary>
    public AppRepeater()
        => InitializeComponent();

    /// <summary>
    /// 列表数据源.
    /// </summary>
    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// 内部列表的键盘焦点导航模式.
    /// </summary>
    public KeyboardNavigationMode RepeaterTabFocusNavigation
    {
        get => (KeyboardNavigationMode)GetValue(RepeaterTabFocusNavigationProperty);
        set => SetValue(RepeaterTabFocusNavigationProperty, value);
    }

    /// <summary>
    /// 数据模板选择器.
    /// </summary>
    public DataTemplateSelector? ItemTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(ItemTemplateSelectorProperty);
        set => SetValue(ItemTemplateSelectorProperty, value);
    }

    /// <summary>
    /// 数据模板.
    /// </summary>
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// 项容器模板，优先于默认的内容呈现模板.
    /// </summary>
    public DataTemplate? ItemContainerTemplate
    {
        get => (DataTemplate?)GetValue(ItemContainerTemplateProperty);
        set => SetValue(ItemContainerTemplateProperty, value);
    }

    /// <summary>
    /// 列表布局.
    /// </summary>
    public VirtualizingLayout? Layout
    {
        get => (VirtualizingLayout?)GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => (sender as AppRepeater)?.SetCollection();

    private void SetCollection()
        => PresenterCollection.Source = ItemsSource as IEnumerable<object>;

    private DataTemplate? GetItemTemplate(DataTemplate? itemContainerTemplate)
        => itemContainerTemplate ?? ContentPresenterTemplate;
}

/// <summary>
/// 带拉伸属性的 ItemsRepeater.
/// </summary>
public sealed class AppItemsRepeater : ItemsRepeater
{
    /// <summary>
    /// 标识 Stretch 依赖属性.
    /// </summary>
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
        "Stretch", typeof(bool), typeof(AppItemsRepeater), new PropertyMetadata(false));

    /// <summary>
    /// 是否拉伸以填满可用空间.
    /// </summary>
    public bool Stretch
    {
        get => (bool)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }
}

/// <summary>
/// 列表项的呈现数据.
/// </summary>
public sealed class ItemContentPresenterData : DependencyObject
{
    /// <summary>
    /// 标识 Content 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        "Content", typeof(object), typeof(ItemContentPresenterData), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ContentTemplate 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
        "ContentTemplate", typeof(DataTemplate), typeof(ItemContentPresenterData), new PropertyMetadata(null));

    /// <summary>
    /// 标识 ContentTemplateSelector 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register(
        "ContentTemplateSelector", typeof(DataTemplateSelector), typeof(ItemContentPresenterData), new PropertyMetadata(null));

    /// <summary>
    /// 标识 PositionInSet 依赖属性.
    /// </summary>
    public static readonly DependencyProperty PositionInSetProperty = DependencyProperty.Register(
        "PositionInSet", typeof(int), typeof(ItemContentPresenterData), new PropertyMetadata(0));

    /// <summary>
    /// 标识 SizeOfSet 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SizeOfSetProperty = DependencyProperty.Register(
        "SizeOfSet", typeof(int), typeof(ItemContentPresenterData), new PropertyMetadata(0));

    /// <summary>
    /// 列表项内容.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// 内容模板.
    /// </summary>
    public DataTemplate? ContentTemplate
    {
        get => (DataTemplate?)GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    /// <summary>
    /// 内容模板选择器.
    /// </summary>
    public DataTemplateSelector? ContentTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(ContentTemplateSelectorProperty);
        set => SetValue(ContentTemplateSelectorProperty, value);
    }

    /// <summary>
    /// 当前项在集合中的位置（从 1 开始）.
    /// </summary>
    public int PositionInSet
    {
        get => (int)GetValue(PositionInSetProperty);
        set => SetValue(PositionInSetProperty, value);
    }

    /// <summary>
    /// 集合中的项数.
    /// </summary>
    public int SizeOfSet
    {
        get => (int)GetValue(SizeOfSetProperty);
        set => SetValue(SizeOfSetProperty, value);
    }

    /// <summary>
    /// 根据内容选择合适的模板.
    /// </summary>
    public DataTemplate? SelectTemplate(object content)
        => ContentTemplateSelector?.SelectTemplate(content) ?? ContentTemplate;
}

/// <summary>
/// 由数据源生成呈现数据的集合.
/// </summary>
public sealed class ItemContentPresenterDataCollection : ObservableCollection<ItemContentPresenterData>
{
    private IEnumerable<object>? _source;

    /// <summary>
    /// 内容模板.
    /// </summary>
    public DataTemplate? ContentTemplate { get; set; }

    /// <summary>
    /// 内容模板选择器.
    /// </summary>
    public DataTemplateSelector? ContentTemplateSelector { get; set; }

    /// <summary>
    /// 原始数据源.
    /// </summary>
    public IEnumerable<object>? Source
    {
        get => _source;
        set
        {
            _source = value;
            Refresh();
        }
    }

    private void Refresh()
    {
        Clear();
        if (_source is null)
        {
            return;
        }

        var items = _source.ToList();
        for (var i = 0; i < items.Count; i++)
        {
            Add(new ItemContentPresenterData
            {
                Content = items[i],
                ContentTemplate = ContentTemplate,
                ContentTemplateSelector = ContentTemplateSelector,
                PositionInSet = i + 1,
                SizeOfSet = items.Count,
            });
        }
    }
}
