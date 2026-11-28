// Copyright (c) Bili Copilot. All rights reserved.

using FluentSymbol = FluentIcons.Common.Symbol;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 带图标与未读标记的导航项.
/// </summary>
public sealed class AppNavigationViewItem : NavigationViewItem
{
    /// <summary>
    /// 标识 Symbol 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        "Symbol", typeof(FluentSymbol), typeof(AppNavigationViewItem), new PropertyMetadata(default(FluentSymbol)));

    /// <summary>
    /// 标识 ShowUnread 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ShowUnreadProperty = DependencyProperty.Register(
        "ShowUnread", typeof(bool), typeof(AppNavigationViewItem), new PropertyMetadata(false));

    /// <summary>
    /// 导航项图标.
    /// </summary>
    public FluentSymbol Symbol
    {
        get => (FluentSymbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// 是否显示未读标记.
    /// </summary>
    public bool ShowUnread
    {
        get => (bool)GetValue(ShowUnreadProperty);
        set => SetValue(ShowUnreadProperty, value);
    }
}

/// <summary>
/// 带图标的导航项呈现器.
/// </summary>
public sealed class AppNavigationViewItemPresenter : NavigationViewItemPresenter
{
    /// <summary>
    /// 标识 Symbol 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        "Symbol", typeof(FluentSymbol), typeof(AppNavigationViewItemPresenter), new PropertyMetadata(default(FluentSymbol)));

    /// <summary>
    /// 导航项图标.
    /// </summary>
    public FluentSymbol Symbol
    {
        get => (FluentSymbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
}
