// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 用于布局的控件基类.
/// </summary>
public abstract class LayoutControlBase : Control
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutControlBase"/> class.
    /// </summary>
    protected LayoutControlBase()
    {
        IsTabStop = false;
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
/// 用于布局的控件基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型类型.</typeparam>
public abstract class LayoutControlBase<TViewModel> : LayoutControlBase
    where TViewModel : class
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        "ViewModel",
        typeof(TViewModel),
        typeof(LayoutControlBase<TViewModel>),
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

    /// <summary>
    /// 当视图模型发生变化时，分配值（用于 AOT 赋值）.
    /// </summary>
    protected virtual void AssignValuesWhenViewModelChanged()
    {
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        AssignValuesWhenViewModelChanged();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel is not null)
        {
            AssignValuesWhenViewModelChanged();
        }
    }

    private static void OnViewModelChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LayoutControlBase<TViewModel> obj)
        {
            obj.OnViewModelChanged((TViewModel?)e.OldValue, (TViewModel?)e.NewValue);
            obj.AssignValuesWhenViewModelChanged();
        }
    }
}
