using DBison.Core.EventArguments;
using Newtonsoft.Json;
using System.Configuration;

namespace DBison.Core.Utils.SettingsSystem;
internal static class SettingsHandler
{
    private readonly static Configuration? m_Config;
    static SettingsHandler()
    {
        m_Config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = "app.config" }, ConfigurationUserLevel.None);
    }

    public static event EventHandler<SettingChangedEventArgs> SettingChanged;

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
                return (T)Convert.ChangeType(JsonConvert.DeserializeObject<List<string>>(setting), typeof(List<string>));

            return string.IsNullOrEmpty(setting) ? defaultValue : (T)Convert.ChangeType(setting, typeof(T));
        }
        catch (Exception)
        {
            return defaultValue;
        }

    }

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

            if (m_Config.AppSettings.Settings[key] == null)
                m_Config.AppSettings.Settings.Add(key, value);
            else
                m_Config.AppSettings.Settings[key].Value = value;
            m_Config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(m_Config.AppSettings.SectionInformation.Name);
            SettingChanged(null, new(key));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
