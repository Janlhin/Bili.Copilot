// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 应用版本读取入口.
/// </summary>
internal static class AppVersionToolkit
{
    /// <summary>
    /// 获取应用版本. 未打包进程不能读取 Package.Current，回退到程序集版本.
    /// </summary>
    public static string GetVersion()
    {
        if (PortableToolkit.IsPackaged)
        {
            return GlobalDependencies.Kernel.GetRequiredService<IAppToolkit>().GetPackageVersion();
        }

        var version = typeof(AppVersionToolkit).Assembly.GetName().Version;
        return version is null
            ? "1.0.0.0"
            : $"{version.Major}.{version.Minor}.{Math.Max(version.Build, 0)}.{Math.Max(version.Revision, 0)}";
    }
}
