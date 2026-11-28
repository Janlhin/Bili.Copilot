// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels;

/// <summary>
/// 视图模型基类.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
}

/// <summary>
/// 数据视图模型基类.
/// </summary>
/// <typeparam name="TData">数据类型.</typeparam>
public abstract class ViewModelBase<TData> : ViewModelBase
    where TData : class
{
    private TData _data;

    /// <summary>
    /// 数据.
    /// </summary>
    public TData Data
    {
        get => _data;
        set => SetProperty(ref _data, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase{TData}"/> class.
    /// </summary>
    protected ViewModelBase(TData data)
        => _data = data;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is ViewModelBase<TData> other && EqualityComparer<TData>.Default.Equals(Data, other.Data);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Data);
}
