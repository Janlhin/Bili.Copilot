// Copyright (c) Bili Copilot. All rights reserved.

using System.Runtime.InteropServices;
using System.Text.Json;
using BiliCopilot.UI.Models.Constants;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 设置存储工具. 打包形态沿用 ApplicationData, 未打包形态回退到 JSON 文件.
/// </summary>
internal sealed class SettingsToolkit : ISettingsToolkit
{
    private const string ContainerName = "v1";
    private static readonly object _sync = new();
    private static readonly string SettingsFile = Path.Combine(PortableToolkit.LocalFolderPath, "settings.json");
    private static Microsoft.Windows.Storage.ApplicationDataContainer? _settingContainer;
    private static Dictionary<string, JsonElement>? _fileSettings;

    /// <summary>
    /// Read local setting.
    /// </summary>
    /// <typeparam name="T">Type of read value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="defaultValue">Default value provided when the setting does not exist.</param>
    /// <returns>Setting value obtained.</returns>
    public static T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().ReadLocalSetting(settingName.ToString(), defaultValue);

    /// <summary>
    /// Write local setting.
    /// </summary>
    /// <typeparam name="T">Type of written value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="value">Setting value.</param>
    public static void WriteLocalSetting<T>(SettingNames settingName, T value)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().WriteLocalSetting(settingName.ToString(), value);

    /// <summary>
    /// Delete local setting.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    public static void DeleteLocalSetting(SettingNames settingName)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().DeleteLocalSetting(settingName.ToString());

    /// <summary>
    /// Whether the setting to be read has been created locally.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    /// <returns><c>true</c> means the local setting exists, <c>false</c> means it does not exist.</returns>
    public static bool IsSettingKeyExist(SettingNames settingName)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().IsSettingKeyExist(settingName.ToString());

    /// <inheritdoc/>
    public T ReadLocalSetting<T>(string settingName, T defaultValue)
    {
        if (TryGetContainer(out var container))
        {
            if (container.Values.TryGetValue(settingName, out var value) && value is not null)
            {
                return JsonSerializer.Deserialize<T>(value.ToString()!) ?? defaultValue;
            }

            return defaultValue;
        }

        lock (_sync)
        {
            var settings = GetFileSettings();
            return settings.TryGetValue(settingName, out var element)
                ? element.Deserialize<T>() ?? defaultValue
                : defaultValue;
        }
    }

    /// <inheritdoc/>
    public void WriteLocalSetting<T>(string settingName, T value)
    {
        if (TryGetContainer(out var container))
        {
            container.Values[settingName] = JsonSerializer.Serialize(value);
            return;
        }

        lock (_sync)
        {
            var settings = GetFileSettings();
            settings[settingName] = JsonSerializer.SerializeToElement(value);
            SaveFileSettings(settings);
        }
    }

    /// <inheritdoc/>
    public void DeleteLocalSetting(string settingName)
    {
        if (TryGetContainer(out var container))
        {
            container.Values.Remove(settingName);
            return;
        }

        lock (_sync)
        {
            var settings = GetFileSettings();
            if (settings.Remove(settingName))
            {
                SaveFileSettings(settings);
            }
        }
    }

    /// <inheritdoc/>
    public bool IsSettingKeyExist(string settingName)
    {
        if (TryGetContainer(out var container))
        {
            return container.Values.ContainsKey(settingName);
        }

        lock (_sync)
        {
            return GetFileSettings().ContainsKey(settingName);
        }
    }

    private static bool TryGetContainer(out Microsoft.Windows.Storage.ApplicationDataContainer container)
    {
        if (_settingContainer is not null)
        {
            container = _settingContainer;
            return true;
        }

        try
        {
            _settingContainer = Microsoft.Windows.Storage.ApplicationData.GetDefault()
                .LocalSettings.CreateContainer(ContainerName, Microsoft.Windows.Storage.ApplicationDataCreateDisposition.Always);
            container = _settingContainer;
            return true;
        }
        catch (COMException)
        {
            container = null!;
            return false;
        }
    }

    private static Dictionary<string, JsonElement> GetFileSettings()
    {
        if (_fileSettings is not null)
        {
            return _fileSettings;
        }

        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = File.ReadAllText(SettingsFile);
                _fileSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? [];
                return _fileSettings;
            }
        }
        catch (JsonException)
        {
            // 设置文件损坏时按空配置启动, 避免应用无法进入.
        }
        catch (IOException)
        {
            // 文件被占用时按空配置启动.
        }

        _fileSettings = [];
        return _fileSettings;
    }

    private static void SaveFileSettings(Dictionary<string, JsonElement> settings)
    {
        var directory = Path.GetDirectoryName(SettingsFile)!;
        Directory.CreateDirectory(directory);
        var tempFile = SettingsFile + ".tmp";
        File.WriteAllText(tempFile, JsonSerializer.Serialize(settings));
        File.Move(tempFile, SettingsFile, true);
    }
}
