// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 用于布局的用户控件基类.
/// </summary>
public abstract class LayoutUserControlBase : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutUserControlBase"/> class.
    /// </summary>
    protected LayoutUserControlBase()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 控件加载完成.
    /// </summary>
    protected virtual void OnControlLoaded()
    {
    }

    /// <summary>
    /// 控件卸载完成.
    /// </summary>
    protected virtual void OnControlUnloaded()
    {
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => OnControlLoaded();

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => OnControlUnloaded();
}

/// <summary>
/// 用于布局的用户控件基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型类型.</typeparam>
public abstract class LayoutUserControlBase<TViewModel> : LayoutUserControlBase
    where TViewModel : class
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        "ViewModel",
        typeof(TViewModel),
        typeof(LayoutUserControlBase<TViewModel>),
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
        => (d as LayoutUserControlBase<TViewModel>)?.OnViewModelChanged((TViewModel?)e.OldValue, (TViewModel?)e.NewValue);
}
