// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace BiliCopilot.UI.Controls.Local;

/// <summary>
/// 应用标题栏，支持返回按钮、标题/副标题及左右附加内容.
/// </summary>
[ContentProperty(Name = "Content")]
public sealed class AppTitleBar : LayoutControlBase
{
    private const string BackButtonPartName = "PART_BackButton";
    private const string PaneToggleButtonPartName = "PART_PaneToggleButton";
    private const string HeaderPartName = "PART_HeaderContentPresenter";
    private const string FooterPartName = "PART_FooterContentPresenter";
    private const string LeftEdgePartName = "PART_LeftEdgeContentPresenter";
    private const string CenterPartName = "PART_CenterPresenter";
    private const string ContentPartName = "PART_ContentPresenter";

    private ColumnDefinition? _leftPaddingColumn;
    private ColumnDefinition? _rightPaddingColumn;
    private Button? _backButton;
    private Button? _paneToggleButton;

    /// <summary>
    /// 标识 Header 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        "Header", typeof(object), typeof(AppTitleBar), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// 标识 LeftEdgeElement 依赖属性.
    /// </summary>
    public static readonly DependencyProperty LeftEdgeElementProperty = DependencyProperty.Register(
        "LeftEdgeElement", typeof(object), typeof(AppTitleBar), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// 标识 IconElement 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconElementProperty = DependencyProperty.Register(
        "IconElement", typeof(IconElement), typeof(AppTitleBar), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// 标识 Title 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title", typeof(string), typeof(AppTitleBar), new PropertyMetadata(string.Empty, OnPropertyChanged));

    /// <summary>
    /// 标识 Subtitle 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        "Subtitle", typeof(string), typeof(AppTitleBar), new PropertyMetadata(string.Empty, OnPropertyChanged));

    /// <summary>
    /// 标识 Content 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        "Content", typeof(object), typeof(AppTitleBar), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// 标识 Footer 依赖属性.
    /// </summary>
    public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
        "Footer", typeof(object), typeof(AppTitleBar), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// 标识 IsBackButtonVisible 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register(
        "IsBackButtonVisible", typeof(bool), typeof(AppTitleBar), new PropertyMetadata(false, OnPropertyChanged));

    /// <summary>
    /// 标识 IsBackEnabled 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsBackEnabledProperty = DependencyProperty.Register(
        "IsBackEnabled", typeof(bool), typeof(AppTitleBar), new PropertyMetadata(true, OnPropertyChanged));

    /// <summary>
    /// 标识 IsPaneToggleButtonVisible 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsPaneToggleButtonVisibleProperty = DependencyProperty.Register(
        "IsPaneToggleButtonVisible", typeof(bool), typeof(AppTitleBar), new PropertyMetadata(false, OnPropertyChanged));

    /// <summary>
    /// 标识 TemplateSettings 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TemplateSettingsProperty = DependencyProperty.Register(
        "TemplateSettings", typeof(AppTitleBarTemplateSettings), typeof(AppTitleBar), new PropertyMetadata(null));

    /// <summary>
    /// 标识 TitleMaxWidth 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleMaxWidthProperty = DependencyProperty.Register(
        "TitleMaxWidth", typeof(double), typeof(AppTitleBar), new PropertyMetadata(double.PositiveInfinity));

    /// <summary>
    /// 标识 CenterContent 依赖属性.
    /// </summary>
    public static readonly DependencyProperty CenterContentProperty = DependencyProperty.Register(
        "CenterContent", typeof(object), typeof(AppTitleBar), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// 标识 TitleForeground 依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(
        "TitleForeground", typeof(Brush), typeof(AppTitleBar), new PropertyMetadata(null));

    /// <summary>
    /// 标识 AutoUpdateTitleBarButtonColor 依赖属性.
    /// </summary>
    public static readonly DependencyProperty AutoUpdateTitleBarButtonColorProperty = DependencyProperty.Register(
        "AutoUpdateTitleBarButtonColor", typeof(bool), typeof(AppTitleBar), new PropertyMetadata(true));

    /// <summary>
    /// 初始化 <see cref="AppTitleBar"/> 的新实例.
    /// </summary>
    public AppTitleBar()
        => DefaultStyleKey = typeof(AppTitleBar);

    /// <summary>
    /// 请求返回.
    /// </summary>
    public event EventHandler? BackRequested;

    /// <summary>
    /// 请求切换导航窗格.
    /// </summary>
    public event EventHandler? PaneToggleRequested;

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>
    /// 标题前景色.
    /// </summary>
    public Brush? TitleForeground
    {
        get => (Brush?)GetValue(TitleForegroundProperty);
        set => SetValue(TitleForegroundProperty, value);
    }

    /// <summary>
    /// 标题最大宽度.
    /// </summary>
    public double TitleMaxWidth
    {
        get => (double)GetValue(TitleMaxWidthProperty);
        set => SetValue(TitleMaxWidthProperty, value);
    }

    /// <summary>
    /// 是否显示返回按钮.
    /// </summary>
    public bool IsBackButtonVisible
    {
        get => (bool)GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    /// <summary>
    /// 返回按钮是否可用.
    /// </summary>
    public bool IsBackEnabled
    {
        get => (bool)GetValue(IsBackEnabledProperty);
        set => SetValue(IsBackEnabledProperty, value);
    }

    /// <summary>
    /// 是否显示导航切换按钮.
    /// </summary>
    public bool IsPaneToggleButtonVisible
    {
        get => (bool)GetValue(IsPaneToggleButtonVisibleProperty);
        set => SetValue(IsPaneToggleButtonVisibleProperty, value);
    }

    /// <summary>
    /// 标题栏图标.
    /// </summary>
    public IconElement? IconElement
    {
        get => (IconElement?)GetValue(IconElementProperty);
        set => SetValue(IconElementProperty, value);
    }

    /// <summary>
    /// 标题栏头部内容.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// 标题栏底部内容.
    /// </summary>
    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// 标题栏左侧内容.
    /// </summary>
    public object? LeftEdgeElement
    {
        get => GetValue(LeftEdgeElementProperty);
        set => SetValue(LeftEdgeElementProperty, value);
    }

    /// <summary>
    /// 标题栏中间内容.
    /// </summary>
    public object? CenterContent
    {
        get => GetValue(CenterContentProperty);
        set => SetValue(CenterContentProperty, value);
    }

    /// <summary>
    /// 标题栏主体内容.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// 模板设置.
    /// </summary>
    public AppTitleBarTemplateSettings? TemplateSettings
    {
        get => (AppTitleBarTemplateSettings?)GetValue(TemplateSettingsProperty);
        set => SetValue(TemplateSettingsProperty, value);
    }

    /// <summary>
    /// 是否自动更新标题栏按钮颜色.
    /// </summary>
    public bool AutoUpdateTitleBarButtonColor
    {
        get => (bool)GetValue(AutoUpdateTitleBarButtonColorProperty);
        set => SetValue(AutoUpdateTitleBarButtonColorProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_backButton is not null)
        {
            _backButton.Click -= OnBackButtonClick;
        }

        if (_paneToggleButton is not null)
        {
            _paneToggleButton.Click -= OnPaneToggleButtonClick;
        }

        _leftPaddingColumn = GetTemplateChild("LeftPaddingColumn") as ColumnDefinition;
        _rightPaddingColumn = GetTemplateChild("RightPaddingColumn") as ColumnDefinition;
        _backButton = GetTemplateChild(BackButtonPartName) as Button;
        _paneToggleButton = GetTemplateChild(PaneToggleButtonPartName) as Button;

        if (_backButton is not null)
        {
            _backButton.Click += OnBackButtonClick;
        }

        if (_paneToggleButton is not null)
        {
            _paneToggleButton.Click += OnPaneToggleButtonClick;
        }

        TemplateSettings ??= new AppTitleBarTemplateSettings();
        UpdateStates();
        UpdatePadding();
    }

    /// <summary>
    /// 更新标题栏左右内边距，使其与系统窗口按钮对齐.
    /// </summary>
    public void UpdatePadding()
    {
        var environment = XamlRoot?.ContentIslandEnvironment;
        if (environment is null)
        {
            return;
        }

        try
        {
            var titleBar = AppWindow.GetFromWindowId(environment.AppWindowId).TitleBar;
            var scale = XamlRoot?.RasterizationScale ?? 1d;
            if (_leftPaddingColumn is not null)
            {
                _leftPaddingColumn.Width = new GridLength(titleBar.LeftInset / scale, GridUnitType.Pixel);
            }

            if (_rightPaddingColumn is not null)
            {
                _rightPaddingColumn.Width = new GridLength(titleBar.RightInset / scale, GridUnitType.Pixel);
            }
        }
        catch (Exception)
        {
            // 无包标识或临时窗口状态下 AppWindow 可能不可用，此时保留原内边距.
        }
    }

    /// <summary>
    /// 延迟更新标题栏内边距，等待窗口完成布局.
    /// </summary>
    public async void DelayUpdateAsync()
    {
        await Task.Delay(100);
        UpdatePadding();
    }

    private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => (sender as AppTitleBar)?.UpdateStates();

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => BackRequested?.Invoke(this, EventArgs.Empty);

    private void OnPaneToggleButtonClick(object sender, RoutedEventArgs e)
        => PaneToggleRequested?.Invoke(this, EventArgs.Empty);

    private void UpdateStates()
    {
        GoToState(IsBackButtonVisible ? "BackButtonVisible" : "BackButtonCollapsed");
        GoToState(IsPaneToggleButtonVisible ? "PaneToggleButtonVisible" : "PaneToggleButtonCollapsed");
        GoToState(Header is null ? "HeaderCollapsed" : "HeaderVisible");
        GoToState(Footer is null ? "FooterCollapsed" : "FooterVisible");
        GoToState(LeftEdgeElement is null ? "LeftEdgeCollapsed" : "LeftEdgeVisible");
        GoToState(string.IsNullOrEmpty(Title) ? "TitleTextCollapsed" : "TitleTextVisible");
        GoToState(string.IsNullOrEmpty(Subtitle) ? "SubtitleTextCollapsed" : "SubtitleTextVisible");
        GoToState(Content is null ? "ContentCollapsed" : "ContentVisible");
        GoToState(CenterContent is null ? "CenterContentCollapsed" : "CenterContentVisible");
        GoToState(IconElement is null ? "IconCollapsed" : "IconVisible");
    }

    private void GoToState(string state)
        => VisualStateManager.GoToState(this, state, true);
}
