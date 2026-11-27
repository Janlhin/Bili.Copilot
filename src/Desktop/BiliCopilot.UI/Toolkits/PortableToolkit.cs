// Copyright (c) Bili Copilot. All rights reserved.

using System.Runtime.InteropServices;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 无包标识运行时的路径兼容层.
/// 打包形态保持原行为, 未打包形态回退到 %LOCALAPPDATA%\BiliCopilot\ 与程序目录.
/// </summary>
internal static class PortableToolkit
{
    private static readonly string FallbackRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "BiliCopilot");

    /// <summary>
    /// 当前进程是否为打包应用.
    /// </summary>
    public static bool IsPackaged
    {
        get
        {
            try
            {
                _ = Windows.ApplicationModel.Package.Current;
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 应用本地数据目录, 无包标识时回退到用户目录.
    /// </summary>
    public static string LocalFolderPath
    {
        get
        {
            try
            {
                var path = Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path;
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
            }
            catch (COMException)
            {
                // 无包标识时 ApplicationData 不可用, 走下面的回退分支.
            }

            Directory.CreateDirectory(FallbackRoot);
            return FallbackRoot;
        }
    }

    /// <summary>
    /// 应用安装目录, 无包标识时回退到可执行文件所在目录.
    /// </summary>
    public static string InstalledPath
    {
        get
        {
            try
            {
                var path = Windows.ApplicationModel.Package.Current.InstalledPath;
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
            }
            catch (COMException)
            {
                // 无包标识时 Package.Current 不可用, 走下面的回退分支.
            }

            return AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}
