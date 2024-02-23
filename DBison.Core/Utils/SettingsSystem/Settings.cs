using DBison.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.Core.Utils.SettingsSystem;

public static class Settings
{
    [Setting("Visual", "Font Size", typeof(uint))]
    [Range(10, 100)]
    public static uint FontSize
    {
        get => SettingsHandler.GetSetting("FontSize", 12u);
        set => SettingsHandler.SetSetting("FontSize", value.ToString());
    }

    [Setting("Visual", "Use Dark Mode", typeof(bool))]
    public static bool UseDarkMode
    {
        get => SettingsHandler.GetSetting("UseDarkMode", false);
        set => SettingsHandler.SetSetting("UseDarkMode", value.ToString());
    }

    [Setting("Performance", "Timeout", typeof(int))]
    [Range(-1, int.MaxValue)]
    public static int Timeout
    {
        get => SettingsHandler.GetSetting("Timeout", 30);
        set => SettingsHandler.SetSetting("Timeout", value.ToString());
    }

    [Setting("Performance", "Limit", typeof(int))]
    [Range(-1, int.MaxValue)]
    public static int Limit
    {
        get => SettingsHandler.GetSetting("Limit", 100);
        set => SettingsHandler.SetSetting("Limit", value.ToString());
    }

    [Setting("Visual", "Use Translucent Window", typeof(bool))]
    public static bool UseTranslucentWindow
    {
        get => SettingsHandler.GetSetting("UseTranslucentWindow", false);
        set => SettingsHandler.SetSetting("UseTranslucentWindow", value.ToString());
    }

    [Setting("Visual", "Translucent Opacity", typeof(uint))]
    [Range(10, 100)]
    public static uint TranslucentOpacity
    {
        get => SettingsHandler.GetSetting("TranslucentOpacity", 100u);
        set => SettingsHandler.SetSetting("TranslucentOpacity", value.ToString());
    }

    [Setting("Visual", "UI Scaling", typeof(double))]
    [Range(0.1, 2.0)]
    public static double UIScaling
    {
        get => SettingsHandler.GetSetting("UIScaling", 1.0);
        set => SettingsHandler.SetSetting("UIScaling", value.ToString());
    }
    
    [Setting("Performance", "Use Session Store", typeof(bool))]
    public static bool UseSessionStore
    {
        get => SettingsHandler.GetSetting("UseSessionStore", false);
        set => SettingsHandler.SetSetting("UseSessionStore", value.ToString());
    }

    [Setting("Performance", "Max Backup Count", typeof(int))]
    [Range(-1, 100)]
    public static int MaxBackupCount
    {
        get => SettingsHandler.GetSetting("MaxBackupCount", -1);
        set => SettingsHandler.SetSetting("MaxBackupCount", value.ToString());
    }

    [Setting("Performance", "Max Log Size", typeof(double))]
    [Range(-1.0, 100.0)]
    public static double MaxLogSize
    {
        get => SettingsHandler.GetSetting("MaxLogSize", -1.0);
        set => SettingsHandler.SetSetting("MaxLogSize", value.ToString());
    }

    [Setting("Database", "Show Extended Database Information", typeof(bool))]
    public static bool ShowExtendedDatabaseInformation
    {
        get => SettingsHandler.GetSetting("ShowExtendedDatabaseInformation", false);
        set => SettingsHandler.SetSetting("ShowExtendedDatabaseInformation", value.ToString());
    }

    [Setting("Database", "Open Query On Server Added", typeof(bool))]
    public static bool OpenQueryOnServerAdded
    {
        get => SettingsHandler.GetSetting("OpenQueryOnServerAdded", true);
        set => SettingsHandler.SetSetting("OpenQueryOnServerAdded", value.ToString());
    }

    [Setting("Database", "Deactivation of DML list", typeof(List<string>))]
    public static List<string> DeactionDMLList
    {
        get => SettingsHandler.GetSetting("DeactionDMLList", new List<string>());
        set => SettingsHandler.SetSetting("DeactionDMLList", JsonConvert.SerializeObject(value));
    }
    public static string GetAllSettingsString()
    {
        var props = typeof(Settings).GetProperties();
        var sb = new StringBuilder();
        foreach (var prop in props)
        {
            sb.AppendLine($"{prop.Name}: {prop.GetValue(null)}");
        }
        return sb.ToString();
    }
}
