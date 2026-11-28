// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 为样式提供注入资源字典的附加属性，供设置卡片等控件模板使用.
/// </summary>
public static class StyleExtensions
{
    private sealed class StyleExtensionResourceDictionary : ResourceDictionary
    {
    }

    /// <summary>
    /// 标识 Resources 附加属性.
    /// </summary>
    public static readonly DependencyProperty ResourcesProperty = DependencyProperty.RegisterAttached(
        "Resources",
        typeof(ResourceDictionary),
        typeof(StyleExtensions),
        new PropertyMetadata(null, OnResourcesChanged));

    /// <summary>
    /// 获取附加的资源字典.
    /// </summary>
    public static ResourceDictionary? GetResources(Style obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (ResourceDictionary?)obj.GetValue(ResourcesProperty);
    }

    /// <summary>
    /// 设置附加的资源字典.
    /// </summary>
    public static void SetResources(Style obj, ResourceDictionary? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(ResourcesProperty, value);
    }

    private static void OnResourcesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not FrameworkElement element || element.Resources is null)
        {
            return;
        }

        var merged = element.Resources.MergedDictionaries;
        var existing = merged.FirstOrDefault(x => x is StyleExtensionResourceDictionary);
        if (existing is not null)
        {
            merged.Remove(existing);
        }

        if (args.NewValue is ResourceDictionary source)
        {
            var copy = new StyleExtensionResourceDictionary();
            CopyFrom(copy, source);
            merged.Add(copy);
        }

        if (element.IsLoaded)
        {
            var theme = element.RequestedTheme;
            element.RequestedTheme = theme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            element.RequestedTheme = theme;
        }
    }

    private static void CopyFrom(ResourceDictionary destination, ResourceDictionary source)
    {
        if (source.Source is not null)
        {
            destination.Source = source.Source;
            return;
        }

        foreach (var pair in source.ThemeDictionaries)
        {
            if (pair.Value is ResourceDictionary themeDictionary)
            {
                var copy = new ResourceDictionary();
                CopyFrom(copy, themeDictionary);
                destination.ThemeDictionaries[pair.Key] = copy;
            }
            else
            {
                destination.ThemeDictionaries[pair.Key] = pair.Value;
            }
        }

        foreach (var merged in source.MergedDictionaries)
        {
            var copy = new ResourceDictionary();
            CopyFrom(copy, merged);
            destination.MergedDictionaries.Add(copy);
        }

        foreach (var pair in source)
        {
            destination[pair.Key] = pair.Value;
        }
    }
}

/// <summary>
/// 设置展开项选择卡片样式的选择器.
/// </summary>
public sealed class SettingsExpanderItemStyleSelector : StyleSelector
{
    /// <summary>
    /// 默认样式.
    /// </summary>
    public Style? DefaultStyle { get; set; }

    /// <summary>
    /// 可点击时的样式.
    /// </summary>
    public Style? ClickableStyle { get; set; }

    /// <inheritdoc/>
    protected override Style? SelectStyleCore(object item, DependencyObject container)
        => container is SettingsCard { IsClickEnabled: true } ? ClickableStyle : DefaultStyle;
}

/// <summary>
/// 应用标题栏的模板设置.
/// </summary>
public sealed class AppTitleBarTemplateSettings : DependencyObject
{
    /// <summary>
    /// 标识 IconElement 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconElementProperty = DependencyProperty.Register(
        "IconElement",
        typeof(IconElement),
        typeof(AppTitleBarTemplateSettings),
        new PropertyMetadata(null));

    /// <summary>
    /// 标题栏图标.
    /// </summary>
    public IconElement? IconElement
    {
        get => (IconElement?)GetValue(IconElementProperty);
        set => SetValue(IconElementProperty, value);
    }
}

/// <summary>
/// 不参与虚拟化的堆叠布局，供列表控件使用.
/// </summary>
public sealed class NonVirtualizingStackLayout : VirtualizingLayout
{
    /// <summary>
    /// 标识 Orientation 依赖属性.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        "Orientation",
        typeof(Orientation),
        typeof(NonVirtualizingStackLayout),
        new PropertyMetadata(Orientation.Vertical));

    /// <summary>
    /// 标识 Spacing 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        "Spacing",
        typeof(double),
        typeof(NonVirtualizingStackLayout),
        new PropertyMetadata(0d));

    /// <summary>
    /// 排列方向.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// 元素间距.
    /// </summary>
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }
}
