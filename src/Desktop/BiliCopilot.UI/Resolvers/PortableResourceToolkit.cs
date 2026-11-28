// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Media;
using Richasy.WinUIKernel.Share.Toolkits;
using System.Xml.Linq;

namespace BiliCopilot.UI.Resolvers;

/// <summary>
/// 无包标识（Portable）运行时使用的资源管理器。
/// WinUI 默认的 ResourceLoader 在没有包标识时会触发 0x80073D54 的 WinRT 异常，
/// 并在 .NET 10 下升级为致命的 stowed exception。这里直接从随输出分发的 .resw 读取文本。
/// </summary>
internal sealed class PortableResourceToolkit : IResourceToolkit
{
    private static readonly Dictionary<string, string> Strings = LoadStrings();

    public string GetLocalizedString(string name)
        => Strings.TryGetValue(name, out var value) ? value : name;

    public Brush GetThemeBrush(string key)
    {
        if (Application.Current is not null
            && Application.Current.Resources.TryGetValue(key, out var value)
            && value is Brush brush)
        {
            return brush;
        }

        return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }

    private static Dictionary<string, string> LoadStrings()
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Resources.resw");
            if (!File.Exists(path))
            {
                return result;
            }

            var doc = XDocument.Load(path);
            foreach (var data in doc.Root?.Elements("data") ?? [])
            {
                var name = data.Attribute("name")?.Value;
                var value = data.Element("value")?.Value;
                if (!string.IsNullOrEmpty(name) && value is not null)
                {
                    result[name] = value;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PortableResource: {ex.Message}");
        }

        return result;
    }
}
