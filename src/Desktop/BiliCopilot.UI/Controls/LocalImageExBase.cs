// Copyright (c) Bili Copilot. All rights reserved.

using System.Security.Cryptography;
using System.Text;
using BiliCopilot.UI.Toolkits;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 图片控件本地基类.
/// </summary>
/// <remarks>
/// 与 <see cref="Richasy.WinUIKernel.Share.Base.ImageExBase"/> 行为一致，但磁盘缓存全部使用普通文件 API，
/// 不调用 <c>ApplicationData.GetDefault()</c> 等需要包标识的接口，以便 Portable 免安装形态正常运行。
/// </remarks>
public abstract class LocalImageExBase : Control, IDisposable
{
    private const string CacheRootName = "ImageExCache";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromDays(7);
    private static readonly HttpClient DefaultHttpClient = CreateDefaultHttpClient();
    private static readonly object PendingRequestsLock = new();
    private static readonly Dictionary<string, Task<byte[]?>> PendingRequests = new();

    private CancellationTokenSource _cancellationTokenSource = new();

    private ImageBrush? _backgroundBrush;
    private Uri? _lastUri;
    private long _requestId;
    private bool _disposed;

    /// <summary>
    /// <see cref="Source"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(LocalImageExBase), new PropertyMetadata(null, OnSourceChanged));

    /// <summary>
    /// <see cref="IsShimmerEnabled"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsShimmerEnabledProperty =
        DependencyProperty.Register(nameof(IsShimmerEnabled), typeof(bool), typeof(LocalImageExBase), new PropertyMetadata(true));

    /// <summary>
    /// <see cref="IsImageLoading"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsImageLoadingProperty =
        DependencyProperty.Register(nameof(IsImageLoading), typeof(bool), typeof(LocalImageExBase), new PropertyMetadata(false));

    /// <summary>
    /// <see cref="DecodeWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DecodeWidthProperty =
        DependencyProperty.Register(nameof(DecodeWidth), typeof(double), typeof(LocalImageExBase), new PropertyMetadata(1.0d));

    /// <summary>
    /// <see cref="DecodeHeight"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DecodeHeightProperty =
        DependencyProperty.Register(nameof(DecodeHeight), typeof(double), typeof(LocalImageExBase), new PropertyMetadata(1.0d));

    /// <summary>
    /// <see cref="ClearColor"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ClearColorProperty =
        DependencyProperty.Register(nameof(ClearColor), typeof(Color), typeof(LocalImageExBase), new PropertyMetadata(Colors.Black));

    /// <summary>
    /// <see cref="HolderImage"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty HolderImageProperty =
        DependencyProperty.Register(nameof(HolderImage), typeof(Uri), typeof(LocalImageExBase), new PropertyMetadata(null, OnSourceChanged));

    /// <summary>
    /// <see cref="EnableDiskCache"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty EnableDiskCacheProperty =
        DependencyProperty.Register(nameof(EnableDiskCache), typeof(bool), typeof(LocalImageExBase), new PropertyMetadata(true));

    /// <summary>
    /// 是否输出调试日志（仅 DEBUG 有效）.
    /// </summary>
    public static bool EnableDebugLog { get; set; }

    /// <summary>
    /// 是否在后台线程解码图片.
    /// </summary>
    public static bool EnableBackgroundDecoding { get; set; } = true;

    /// <summary>
    /// 图片源.
    /// </summary>
    public Uri Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// 是否启用加载闪烁.
    /// </summary>
    public bool IsShimmerEnabled
    {
        get => (bool)GetValue(IsShimmerEnabledProperty);
        set => SetValue(IsShimmerEnabledProperty, value);
    }

    /// <summary>
    /// 图片是否正在加载.
    /// </summary>
    public bool IsImageLoading
    {
        get => (bool)GetValue(IsImageLoadingProperty);
        set => SetValue(IsImageLoadingProperty, value);
    }

    /// <summary>
    /// 解码宽度.
    /// </summary>
    public double DecodeWidth
    {
        get => (double)GetValue(DecodeWidthProperty);
        set => SetValue(DecodeWidthProperty, value);
    }

    /// <summary>
    /// 解码高度.
    /// </summary>
    public double DecodeHeight
    {
        get => (double)GetValue(DecodeHeightProperty);
        set => SetValue(DecodeHeightProperty, value);
    }

    /// <summary>
    /// 绘制画布的默认颜色.
    /// </summary>
    public Color ClearColor
    {
        get => (Color)GetValue(ClearColorProperty);
        set => SetValue(ClearColorProperty, value);
    }

    /// <summary>
    /// 占位符图片.
    /// </summary>
    public Uri HolderImage
    {
        get => (Uri)GetValue(HolderImageProperty);
        set => SetValue(HolderImageProperty, value);
    }

    /// <summary>
    /// 是否启用磁盘缓存.
    /// </summary>
    public bool EnableDiskCache
    {
        get => (bool)GetValue(EnableDiskCacheProperty);
        set => SetValue(EnableDiskCacheProperty, value);
    }

    /// <summary>
    /// 图片已加载.
    /// </summary>
    public event EventHandler? ImageLoaded;

    /// <summary>
    /// 图片加载失败.
    /// </summary>
    public event EventHandler? ImageFailed;

    /// <summary>
    /// 初始化 <see cref="LocalImageExBase"/> 的新实例.
    /// </summary>
    protected LocalImageExBase()
    {
        // 使用本地 XAML 样式（Controls/Local/Themes/ImageExBase.xaml，Root 网格 + 本地 Shimmer）.
        DefaultStyleKey = typeof(LocalImageExBase);
        IsTabStop = false;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 已解码的图片画布，由派生类的 <see cref="DrawImage"/> 创建并绘制.
    /// </summary>
    protected CanvasImageSource? CanvasImageSource { get; set; }

    /// <summary>
    /// 获取所有缓存文件的大小.
    /// </summary>
    public ValueTask<long> GetAllCacheSizeAsync()
    {
        var folder = GetCacheRootFolder();
        if (!Directory.Exists(folder))
        {
            return ValueTask.FromResult(0L);
        }

        long size = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
            .Sum(file => new FileInfo(file).Length);
        return ValueTask.FromResult(size);
    }

    /// <summary>
    /// 清除图片缓存文件夹中的所有文件.
    /// </summary>
    public Task ClearCacheAsync()
    {
        var folder = GetCacheRootFolder();
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取缓存文件的路径.
    /// </summary>
    /// <param name="key">源地址.</param>
    /// <param name="cacheSubFolder">缓存子文件夹名称.</param>
    /// <returns>缓存文件路径；不存在或已过期时返回 null.</returns>
    public Task<string?> GetCacheFilePathAsync(string key, string? cacheSubFolder = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            return Task.FromResult<string?>(null);
        }

        var path = Path.Combine(GetCacheFolder(cacheSubFolder), GetCacheFileName(key));
        if (!File.Exists(path))
        {
            return Task.FromResult<string?>(null);
        }

        try
        {
            if (DateTimeOffset.UtcNow - File.GetCreationTimeUtc(path) > CacheExpiration)
            {
                File.Delete(path);
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult<string?>(path);
        }
        catch (IOException)
        {
            return Task.FromResult<string?>(null);
        }
        catch (UnauthorizedAccessException)
        {
            return Task.FromResult<string?>(null);
        }
    }

    /// <summary>
    /// 写入缓存文件.
    /// </summary>
    public async Task WriteCacheAsync(string key, byte[] data, string? cacheSubFolder = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key) || data is not { Length: > 0 })
        {
            return;
        }

        var path = Path.Combine(GetCacheFolder(cacheSubFolder), GetCacheFileName(key));
        await File.WriteAllBytesAsync(path, data, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 获取中心裁切区域.
    /// </summary>
    protected static Rect GetCenterCropRect(Rect targetRect, Rect sourceRect)
    {
        var targetRatio = targetRect.Width / targetRect.Height;
        var sourceRatio = sourceRect.Width / sourceRect.Height;
        double width;
        double height;
        if (targetRatio > sourceRatio)
        {
            width = sourceRect.Width;
            height = sourceRect.Width / targetRatio;
        }
        else
        {
            width = sourceRect.Height * targetRatio;
            height = sourceRect.Height;
        }

        return new Rect(
            sourceRect.X + (sourceRect.Width - width) / 2,
            sourceRect.Y + (sourceRect.Height - height) / 2,
            width,
            height);
    }

    /// <summary>
    /// 绘制图片（由派生类实现）.
    /// </summary>
    protected abstract void DrawImage(CanvasBitmap canvasBitmap);

    /// <summary>
    /// 更新占位符图片.
    /// </summary>
    protected virtual void UpdateHolderImage()
    {
    }

    /// <summary>
    /// 获取自定义 <see cref="HttpClient"/> 实例.
    /// </summary>
    protected virtual HttpClient? GetCustomHttpClient()
        => null;

    /// <summary>
    /// 获取缓存子目录.
    /// </summary>
    protected virtual string GetCacheSubFolder()
        => string.Empty;

    /// <summary>
    /// 获取请求头信息.
    /// </summary>
    protected virtual Dictionary<string, string> GetHeaders()
        => new();

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (GetTemplateChild("Root") is Panel root)
        {
            if (root.Background is ImageBrush existingBrush)
            {
                _backgroundBrush = existingBrush;
                return;
            }

            _backgroundBrush = new ImageBrush { Stretch = Stretch.UniformToFill };
            root.Background = _backgroundBrush;
        }
        else if (Background is ImageBrush fallbackBrush)
        {
            _backgroundBrush = fallbackBrush;
        }
        else
        {
            _backgroundBrush = new ImageBrush { Stretch = Stretch.UniformToFill };
            Background = _backgroundBrush;
        }
    }

    /// <summary>
    /// 控件加载完成.
    /// </summary>
    protected virtual void OnControlLoaded()
    {
        if (_backgroundBrush?.ImageSource is null)
        {
            _ = RedrawAsync();
        }
    }

    /// <summary>
    /// 控件卸载.
    /// </summary>
    protected virtual void OnControlUnloaded()
    {
        CancelCurrentRequest();
        CanvasImageSource = null;
        if (_backgroundBrush is not null)
        {
            _backgroundBrush.ImageSource = null;
        }
    }

    /// <summary>
    /// 重绘图片.
    /// </summary>
    protected async Task RedrawAsync()
    {
        _lastUri = null;
        await TryLoadImageAsync(Source).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    private static string GetCacheRootFolder()
        => Path.Combine(PortableToolkit.LocalFolderPath, "Cache", CacheRootName);

    private static string GetCacheFolder(string? cacheSubFolder)
    {
        var folder = GetCacheRootFolder();
        if (!string.IsNullOrEmpty(cacheSubFolder))
        {
            folder = Path.Combine(folder, cacheSubFolder);
        }

        Directory.CreateDirectory(folder);
        return folder;
    }

    private static string GetCacheFileName(string key)
    {
        var normalized = NormalizeKey(key);
        return Convert.ToHexStringLower(MD5.HashData(Encoding.UTF8.GetBytes(normalized)));
    }

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }

        if (!key.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return key.ToLowerInvariant();
        }

        var uri = new Uri(key);
        var result = uri.GetLeftPart(UriPartial.Path).ToLowerInvariant();
        if (string.IsNullOrEmpty(uri.Query))
        {
            return result;
        }

        var query = string.Join("&", uri.Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.ToLowerInvariant())
            .OrderBy(part => part, StringComparer.Ordinal));
        return $"{result}?{query}";
    }

    private static HttpClient CreateDefaultHttpClient()
        => new()
        {
            Timeout = TimeSpan.FromSeconds(30),
            DefaultRequestHeaders =
            {
                { "User-Agent", "WinUIKernel/1.0" },
                { "Accept", "image/*" },
            },
        };

    private static async Task<byte[]?> GetOrCreateRequestAsync(string url, Func<Task<byte[]?>> factory, CancellationToken cancellationToken)
    {
        Task<byte[]> task;
        lock (PendingRequestsLock)
        {
            if (PendingRequests.TryGetValue(url, out var existing))
            {
                task = existing;
            }
            else
            {
                var source = new TaskCompletionSource<byte[]>();
                task = source.Task;
                PendingRequests[url] = task;
                _ = ExecuteRequestAsync(url, factory, source, cancellationToken);
            }
        }

        return await task.ConfigureAwait(false);
    }

    private static async Task ExecuteRequestAsync(string url, Func<Task<byte[]?>> factory, TaskCompletionSource<byte[]> source, CancellationToken cancellationToken)
    {
        try
        {
            var data = await factory().ConfigureAwait(false);
            source.TrySetResult(data ?? Array.Empty<byte>());
        }
        catch (OperationCanceledException)
        {
            source.TrySetCanceled(cancellationToken);
        }
        catch (Exception ex)
        {
            source.TrySetException(ex);
        }
        finally
        {
            lock (PendingRequestsLock)
            {
                PendingRequests.Remove(url);
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => OnControlLoaded();

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => OnControlUnloaded();

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LocalImageExBase control && control.IsLoaded)
        {
            _ = control.RedrawAsync();
        }
    }

    private void CancelCurrentRequest()
    {
        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private (long RequestId, CancellationToken Token) BeginNewRequest()
    {
        CancelCurrentRequest();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        return (Interlocked.Increment(ref _requestId), _cancellationTokenSource.Token);
    }

    private async Task TryLoadImageAsync(Uri? uri)
    {
        uri ??= HolderImage;
        if (_backgroundBrush is null || uri == _lastUri || !IsLoaded || _disposed)
        {
            return;
        }

        _lastUri = uri;
        _backgroundBrush.ImageSource = null;
        var (requestId, token) = BeginNewRequest();

        try
        {
            IsImageLoading = true;
            var cacheSubFolder = GetCacheSubFolder();
            var headers = GetHeaders();
            var bytes = await FetchImageAsync(uri, EnableDiskCache, headers, cacheSubFolder, token).ConfigureAwait(false);
            if (bytes is null || token.IsCancellationRequested || requestId != Interlocked.Read(ref _requestId))
            {
                return;
            }

            var bitmap = await DecodeAsync(bytes).ConfigureAwait(false);
            if (bitmap is null || token.IsCancellationRequested || requestId != Interlocked.Read(ref _requestId))
            {
                bitmap?.Dispose();
                return;
            }

            // DrawImage 使用共享设备与依赖属性，必须回到 UI 线程执行.
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (token.IsCancellationRequested || requestId != Interlocked.Read(ref _requestId))
                    {
                        return;
                    }

                    CanvasImageSource ??= new CanvasImageSource(
                        CanvasDevice.GetSharedDevice(),
                        (float)DecodeWidth,
                        (float)DecodeHeight,
                        96f,
                        CanvasAlphaMode.Premultiplied);
                    DrawImage(bitmap);
                    if (_backgroundBrush is not null)
                    {
                        _backgroundBrush.ImageSource = CanvasImageSource;
                        IsImageLoading = false;
                        ImageLoaded?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[LocalImageExBase] DrawImage failed: {ex}");
                    IsImageLoading = false;
                    ImageFailed?.Invoke(this, EventArgs.Empty);
                }
                finally
                {
                    bitmap.Dispose();
                }
            });
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LocalImageExBase] Load image failed: {ex}");
            IsImageLoading = false;
            ImageFailed?.Invoke(this, EventArgs.Empty);
            if (HolderImage is not null && !token.IsCancellationRequested && !Equals(uri, HolderImage))
            {
                await TryLoadImageAsync(HolderImage).ConfigureAwait(false);
            }
        }
        finally
        {
            if (requestId == Interlocked.Read(ref _requestId) && !token.IsCancellationRequested)
            {
                IsImageLoading = false;
            }
        }
    }

    private async Task<byte[]?> FetchImageAsync(Uri uri, bool enableDiskCache, Dictionary<string, string> headers, string cacheSubFolder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (uri.IsFile)
        {
            return await File.ReadAllBytesAsync(uri.LocalPath, cancellationToken).ConfigureAwait(false);
        }

        if (enableDiskCache)
        {
            var cachedPath = await GetCacheFilePathAsync(uri.ToString(), cacheSubFolder).ConfigureAwait(false);
            if (cachedPath is not null)
            {
                return await File.ReadAllBytesAsync(cachedPath, cancellationToken).ConfigureAwait(false);
            }
        }

        var key = uri.ToString();
        var bytes = await GetOrCreateRequestAsync(key, async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var client = GetCustomHttpClient() ?? DefaultHttpClient;
            ApplyHeaders(client, headers, uri);
            using var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            if (data.Length == 0)
            {
                throw new InvalidOperationException("Image content is empty.");
            }

            return data;
        }, cancellationToken).ConfigureAwait(false);

        if (bytes is { Length: > 0 } && enableDiskCache && !cancellationToken.IsCancellationRequested)
        {
            // 缓存写入与图片解码并行，不阻塞 UI.
            _ = WriteCacheSafelyAsync(key, bytes, cacheSubFolder);
        }

        return bytes;
    }

    private async Task WriteCacheSafelyAsync(string key, byte[] data, string cacheSubFolder)
    {
        try
        {
            await WriteCacheAsync(key, data, cacheSubFolder, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LocalImageExBase] Write cache failed: {ex}");
        }
    }

    private void ApplyHeaders(HttpClient client, Dictionary<string, string> headers, Uri uri)
    {
        foreach (var header in headers)
        {
            if (client.DefaultRequestHeaders.Contains(header.Key))
            {
                client.DefaultRequestHeaders.Remove(header.Key);
            }

            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        client.DefaultRequestHeaders.Referrer = uri.Host.Contains("pximg.net", StringComparison.OrdinalIgnoreCase)
            ? new Uri("https://app-api.pixiv.net/")
            : null;
    }

    private static async Task<CanvasBitmap?> DecodeAsync(byte[] bytes)
    {
        var stream = new MemoryStream(bytes).AsRandomAccessStream();
        try
        {
            return await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
        }
        finally
        {
            ((IDisposable)stream)?.Dispose();
        }
    }
}
