// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.ViewModels;
using RichasyInfoType = Richasy.WinUIKernel.Share.Base.InfoType;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel : INotificationViewModel
{
    [RelayCommand]
    private static async Task ShowTipAsync((string, RichasyInfoType) data)
        => await GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().ShowTipCommand.ExecuteAsync(
            (data.Item1, (InfoType)(int)data.Item2));
}
