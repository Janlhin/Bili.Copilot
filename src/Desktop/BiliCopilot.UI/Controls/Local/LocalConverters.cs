// Copyright (c) Bili Copilot. All rights reserved.

using FluentIcons.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 布尔值转可见性.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 是否反转结果.
    /// </summary>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var visible = value is bool flag && flag;
        if (IsReverse)
        {
            visible = !visible;
        }

        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>
/// 对象转布尔值，对象非空（或数值大于零）时为真.
/// </summary>
public sealed class ObjectToBoolConverter : IValueConverter
{
    /// <summary>
    /// 是否反转结果.
    /// </summary>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var result = value switch
        {
            null => false,
            string text => !string.IsNullOrEmpty(text),
            int number => number > 0,
            double number => number > 0d,
            bool flag => flag,
            _ => true,
        };

        return IsReverse ? !result : result;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>
/// 对象转可见性，对象为空时折叠.
/// </summary>
public sealed class ObjectToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// 是否反转结果.
    /// </summary>
    public bool IsReverse { get; set; }

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var nonEmpty = value switch
        {
            null => false,
            string text => !string.IsNullOrEmpty(text),
            int number => number > 0,
            double number => number > 0d,
            _ => true,
        };

        var visible = IsReverse ? !nonEmpty : nonEmpty;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>
/// 布尔值转图标变体.
/// </summary>
public sealed class BoolToIconVariantConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
        => (IconVariant)(value is bool flag && flag ? 1 : 0);

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>
/// 只保留下方两个圆角的圆角转换器，用于展开项的连接样式.
/// </summary>
public sealed class CornerRadiusConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is CornerRadius radius
            ? new CornerRadius(0, 0, radius.BottomRight, radius.BottomLeft)
            : value;

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value;
}
