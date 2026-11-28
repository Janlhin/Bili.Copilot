// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// AI 服务提供方图标.
/// </summary>
public sealed class AILogo : Control
{
    /// <summary>
    /// 标识 Provider 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register(
        "Provider", typeof(string), typeof(AILogo), new PropertyMetadata("OpenAI", OnProviderChanged));

    /// <summary>
    /// 标识 AvatarPadding 依赖属性.
    /// </summary>
    public static readonly DependencyProperty AvatarPaddingProperty = DependencyProperty.Register(
        "AvatarPadding", typeof(Thickness), typeof(AILogo), new PropertyMetadata(new Thickness(6), OnProviderChanged));

    /// <summary>
    /// 标识 IsAvatar 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsAvatarProperty = DependencyProperty.Register(
        "IsAvatar", typeof(bool), typeof(AILogo), new PropertyMetadata(false, OnProviderChanged));

    private Grid? _avatarContainer;
    private Image? _logo;

    /// <summary>
    /// 初始化 <see cref="AILogo"/> 的新实例.
    /// </summary>
    public AILogo()
        => DefaultStyleKey = typeof(AILogo);

    /// <summary>
    /// 服务提供方名称，用于定位图标资源.
    /// </summary>
    public string Provider
    {
        get => (string)GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

    /// <summary>
    /// 头像模式下的内边距.
    /// </summary>
    public Thickness AvatarPadding
    {
        get => (Thickness)GetValue(AvatarPaddingProperty);
        set => SetValue(AvatarPaddingProperty, value);
    }

    /// <summary>
    /// 是否使用圆形头像样式.
    /// </summary>
    public bool IsAvatar
    {
        get => (bool)GetValue(IsAvatarProperty);
        set => SetValue(IsAvatarProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _avatarContainer = GetTemplateChild("AvatarContainer") as Grid;
        _logo = GetTemplateChild("Logo") as Image;
        ResetLogo();
    }

    private static void OnProviderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => (sender as AILogo)?.ResetLogo();

    private void ResetLogo()
    {
        if (_avatarContainer is null || _logo is null)
        {
            return;
        }

        var provider = (Provider ?? string.Empty).ToLowerInvariant();
        var suffix = IsAvatar
            ? "avatar"
            : ActualTheme == ElementTheme.Dark ? "dark" : "light";
        var path = $"ms-appx:///Assets/Providers/{provider}-{suffix}.png";
        _logo.Source = new BitmapImage(new Uri(path));
        _avatarContainer.Padding = IsAvatar ? AvatarPadding : new Thickness(0);
        VisualStateManager.GoToState(this, IsAvatar ? "AvatarState" : "FullState", false);
    }
}
