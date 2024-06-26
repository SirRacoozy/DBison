﻿using DBison.Core.EventArguments;
using Newtonsoft.Json;
using System.Configuration;

namespace DBison.Core.Utils.SettingsSystem;
public static class SettingsHandler
{

    #region - needs -
    private readonly static Configuration? m_Config;
    private readonly static string m_ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DBison");
    private readonly static string m_ConfigPath = Path.Combine(m_ConfigDirectory, "app.config");

    #endregion

    #region [SettingsHandler]
    static SettingsHandler()
    {
        if (!Directory.Exists(m_ConfigDirectory))
            Directory.CreateDirectory(m_ConfigDirectory);
        m_Config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = m_ConfigPath }, ConfigurationUserLevel.None);
    }
    #endregion

    public static event EventHandler<SettingChangedEventArgs> SettingChanged;

    #region [GetSetting]
    /// <summary>
    /// Retrieves a setting from the application's configuration.
    /// </summary>
    /// <param name="key">The key of the setting to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the setting is not found or an error occurs.</param>
    /// <returns>The value of the setting if found and successfully converted to the type T, otherwise the default value.</returns>
    internal static T GetSetting<T>(string key, T defaultValue)
    {
        try
        {
            if (m_Config == null)
                return defaultValue;

            if (m_Config.AppSettings.Settings[key] == null)
                m_Config.AppSettings.Settings.Add(key, defaultValue.ToString());

            var setting = m_Config.AppSettings.Settings[key].Value;

            if (typeof(T) == typeof(List<string>))
            {
                return (T)Convert.ChangeType(JsonConvert.DeserializeObject<List<string>>(setting), typeof(List<string>));
            }
            else if (typeof(T).IsEnum)
            {
                Enum.TryParse(typeof(T), setting, true, out object myEnum);
                return (T)myEnum;
            }
            return string.IsNullOrEmpty(setting) ? defaultValue : (T)Convert.ChangeType(setting, typeof(T));
        }
        catch (Exception)
        {
            return defaultValue;
        }

    }
    #endregion

    #region [SetSetting]
    /// <summary>
    /// Sets a setting in the application's configuration.
    /// </summary>
    /// <param name="key">The key of the setting to set.</param>
    /// <param name="value">The value to set the setting to.</param>
    /// <returns>True if the setting was successfully set, false otherwise.</returns>
    internal static bool SetSetting(string key, string value)
    {
        try
        {
            if (m_Config == null)
                return false;

            if (key == nameof(Settings.UIScaling) && Convert.ToDouble(value) < 0.1)
            {
                return false;
            }

            if (m_Config.AppSettings.Settings[key] == null)
                m_Config.AppSettings.Settings.Add(key, value);
            else
                m_Config.AppSettings.Settings[key].Value = value;
            m_Config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(m_Config.AppSettings.SectionInformation.Name);
            SettingChanged?.Invoke(null, new(key));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    #endregion

}
