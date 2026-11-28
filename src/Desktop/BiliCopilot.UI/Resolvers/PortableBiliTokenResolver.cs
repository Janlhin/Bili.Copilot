// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Authorization;
using System.Text.Json;

namespace BiliCopilot.UI.Resolvers;

/// <summary>
/// 无包标识（Portable）运行时使用的文件令牌解析器。
/// WinUI 默认解析器依赖 ApplicationData.Current，在未打包进程中会触发
/// 0x80073D54 的 WinRT 异常（.NET 10 下会进一步升级为致命的 stowed exception）。
/// </summary>
internal sealed class PortableBiliTokenResolver : IBiliTokenResolver
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private static string TokenPath => Path.Combine(PortableToolkit.LocalFolderPath, "bili-token.json");

    public BiliToken? GetToken()
    {
        try
        {
            var path = TokenPath;
            if (!File.Exists(path))
            {
                return null;
            }

            return JsonSerializer.Deserialize<BiliToken>(File.ReadAllText(path), JsonOptions);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void SaveToken(BiliToken token)
    {
        try
        {
            var path = TokenPath;
            var folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(path, JsonSerializer.Serialize(token, JsonOptions));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PortableToken: {ex.Message}");
        }
    }

    public void RemoveToken()
    {
        try
        {
            File.Delete(TokenPath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PortableToken: {ex.Message}");
        }
    }
}
