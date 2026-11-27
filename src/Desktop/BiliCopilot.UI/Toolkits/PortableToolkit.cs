// Copyright (c) Bili Copilot. All rights reserved.

using System.Reflection;
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
    /// 打包形态使用默认实例; 未打包形态优先使用 GetForUnpackaged,
    /// 若当前 SDK 未提供该 API 则返回 null, 由调用方回退到 %LOCALAPPDATA%\BiliCopilot\.
    /// </summary>
    private static Microsoft.Windows.Storage.ApplicationData? CurrentApplicationData
    {
        get
        {
            try
            {
                if (IsPackaged)
                {
                    return Microsoft.Windows.Storage.ApplicationData.GetDefault();
                }

                // GetForUnpackaged 自 Windows App SDK 2.0 起提供, 当前仓库锁定 1.8,
                // 运行时反射探测可同时兼容 1.8 与 2.0, 避免引用不存在的 API 导致编译失败.
                var method = typeof(Microsoft.Windows.Storage.ApplicationData).GetMethod(
                    "GetForUnpackaged",
                    [typeof(string), typeof(string)]);
                return method?.Invoke(null, ["BiliCopilot", "Portable"]) as Microsoft.Windows.Storage.ApplicationData;
            }
            catch (COMException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (TargetInvocationException)
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
