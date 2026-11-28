// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BiliCopilot.UI.Extensions;

/// <summary>
/// 为 ItemsRepeater 提供更安全的 ItemsSource 设置方式.
/// </summary>
public static class ItemsRepeaterExtension
{
    /// <summary>
    /// 标识 SafeItemsSource 附加属性.
    /// </summary>
    public static readonly DependencyProperty SafeItemsSourceProperty = DependencyProperty.RegisterAttached(
        "SafeItemsSource",
        typeof(System.Collections.IList),
        typeof(ItemsRepeaterExtension),
        new PropertyMetadata(null, OnSafeItemsSourceChanged));

    /// <summary>
    /// 获取 SafeItemsSource.
    /// </summary>
    public static System.Collections.IList? GetSafeItemsSource(ItemsRepeater obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (System.Collections.IList?)obj.GetValue(SafeItemsSourceProperty);
    }

    /// <summary>
    /// 设置 SafeItemsSource.
    /// </summary>
    public static void SetSafeItemsSource(ItemsRepeater obj, System.Collections.IList? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(SafeItemsSourceProperty, value);
    }

    private static void OnSafeItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is ItemsRepeater repeater)
        {
            repeater.ItemsSource = args.NewValue;
        }
    }
}
