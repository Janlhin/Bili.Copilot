// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;

namespace BiliCopilot.UI.Resolvers;

/// <summary>
/// Portable 免安装通道的本人资料服务.
/// Richasy 默认实现及其 HTTP 管道在无包标识进程下会触发 0x80073D54 stowed exception,
/// 因此这里只从本地已保存的登录令牌构造最小资料, 保证主界面能稳定启动.
/// 昵称/头像/等级等联网字段在 Portable 下留空, 不影响导航与播放.
/// </summary>
internal sealed class PortableMyProfileService : IMyProfileService
{
    public Task<UserDetailProfile> GetMyProfileAsync(CancellationToken cancellationToken = default)
    {
        var token = new PortableBiliTokenResolver().GetToken();
        var id = token?.UserId.ToString() ?? "0";
        var user = new UserProfile(id);
        return Task.FromResult(new UserDetailProfile(user));
    }

    public Task<UserCommunityInformation> GetMyCommunityInformationAsync(CancellationToken cancellationToken = default)
    {
        var token = new PortableBiliTokenResolver().GetToken();
        var id = token?.UserId.ToString() ?? "0";
        return Task.FromResult(new UserCommunityInformation(id));
    }
}
