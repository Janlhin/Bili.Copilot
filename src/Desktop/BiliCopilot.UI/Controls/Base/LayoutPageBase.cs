// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 用于布局的页面基类.
/// </summary>
public abstract class LayoutPageBase : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutPageBase"/> class.
    /// </summary>
    protected LayoutPageBase()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 页面加载完成.
    /// </summary>
    protected virtual void OnPageLoaded()
    {
    }

    /// <summary>
    /// 页面卸载完成.
    /// </summary>
    protected virtual void OnPageUnloaded()
    {
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => OnPageLoaded();

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => OnPageUnloaded();
}

/// <summary>
/// 用于布局的页面基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型类型.</typeparam>
public abstract class LayoutPageBase<TViewModel> : LayoutPageBase
    where TViewModel : class
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        "ViewModel",
        typeof(TViewModel),
        typeof(LayoutPageBase<TViewModel>),
        new PropertyMetadata(null, OnViewModelChangedInternal));

    /// <summary>
    /// 视图模型.
    /// </summary>
    public TViewModel ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// 当视图模型发生变化时.
    /// </summary>
    protected virtual void OnViewModelChanged(TViewModel? oldValue, TViewModel? newValue)
    {
    }

    private static void OnViewModelChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => (d as LayoutPageBase<TViewModel>)?.OnViewModelChanged((TViewModel?)e.OldValue, (TViewModel?)e.NewValue);
}
