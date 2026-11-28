// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Richasy.BiliKernel.Bili.Authorization;
using System.Text.Json;

namespace BiliCopilot.UI.Resolvers;

/// <summary>
/// 无包标识（Portable）运行时使用的文件 Cookie 解析器。
/// WinUI 默认解析器依赖 ApplicationData.Current，在未打包进程中会触发
/// 0x80073D54 的 WinRT 异常（.NET 10 下会进一步升级为致命的 stowed exception）。
/// </summary>
internal sealed class PortableBiliCookiesResolver : IBiliCookiesResolver
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private static string CookiesPath => Path.Combine(PortableToolkit.LocalFolderPath, "bili-cookies.json");

    public string GetCookieString()
    {
        var cookies = GetCookies();
        return string.Join("; ", cookies.Select(p => $"{p.Key}={p.Value}"));
    }

    public IDictionary<string, string> GetCookies()
    {
        try
        {
            var path = CookiesPath;
            if (!File.Exists(path))
            {
                return new Dictionary<string, string>();
            }

            return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path), JsonOptions)
                ?? new Dictionary<string, string>();
        }
        catch (Exception)
        {
            return new Dictionary<string, string>();
        }
    }

    public void SaveCookies(IDictionary<string, string> cookies)
    {
        try
        {
            var path = CookiesPath;
            var folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(path, JsonSerializer.Serialize(cookies, JsonOptions));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PortableCookies: {ex.Message}");
        }
    }

    public void RemoveCookies()
    {
        try
        {
            File.Delete(CookiesPath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PortableCookies: {ex.Message}");
        }
    }
}
