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
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 当前应用的 ApplicationData 实例.
    /// 打包形态使用默认实例; 无包标识时返回 null, 由 LocalFolderPath 回退到 %LOCALAPPDATA%\BiliCopilot\.
    /// </summary>
    private static Microsoft.Windows.Storage.ApplicationData? CurrentApplicationData
    {
        get
        {
            if (!IsPackaged)
            {
                return null;
            }

            try
            {
                return Microsoft.Windows.Storage.ApplicationData.GetDefault();
            }
            catch (COMException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
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
                var path = CurrentApplicationData?.LocalFolder.Path;
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
            }
            catch (COMException)
            {
                // 无包标识时 ApplicationData 不可用, 走下面的回退分支.
            }
            catch (InvalidOperationException)
            {
                // 同上.
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
