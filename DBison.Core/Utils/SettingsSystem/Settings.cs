﻿using DBison.Core.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBison.Core.Utils.SettingsSystem;

public static class Settings
{
    [Setting("Visual", "Font Size", "Changes the font size", typeof(uint))]
    [Range(10, 100)]
    public static uint FontSize
    {
        get => SettingsHandler.GetSetting("FontSize", 12u);
        set => SettingsHandler.SetSetting("FontSize", value.ToString());
    }

    [Setting("Visual", "Use Dark Mode", "Activates or deactivates the dark mode", typeof(bool))]
    public static bool UseDarkMode
    {
        get => SettingsHandler.GetSetting("UseDarkMode", false);
        set => SettingsHandler.SetSetting("UseDarkMode", value.ToString());
    }

    [Setting("Performance", "Timeout", "Sets the timeout for any query to the database in seconds", typeof(int))]
    [Range(-1, int.MaxValue)]
    public static int Timeout
    {
        get => SettingsHandler.GetSetting("Timeout", 30);
        set => SettingsHandler.SetSetting("Timeout", value.ToString());
    }

    [Setting("Performance", "Limit", "Limits the default number of results in a single query", typeof(int))]
    [Range(-1, int.MaxValue)]
    public static int Limit
    {
        get => SettingsHandler.GetSetting("Limit", 100);
        set => SettingsHandler.SetSetting("Limit", value.ToString());
    }

    [Setting("Visual", "Use Translucent Window", "Activates the translucency of the window", typeof(bool), isVisible: false)]
    public static bool UseTranslucentWindow
    {
        get => SettingsHandler.GetSetting("UseTranslucentWindow", false);
        set => SettingsHandler.SetSetting("UseTranslucentWindow", value.ToString());
    }

    [Setting("Visual", "Translucent Opacity", "Sets the translucenct opacity", typeof(uint), isVisible: false)]
    [Range(10, 100)]
    public static uint TranslucentOpacity
    {
        get => SettingsHandler.GetSetting("TranslucentOpacity", 100u);
        set => SettingsHandler.SetSetting("TranslucentOpacity", value.ToString());
    }

    [Setting("Visual", "UI Scaling", "Sets the UI scaling between 0.1 and 2.0", typeof(double))]
    [Range(0.1, 2.0)]
    public static double UIScaling
    {
        get => SettingsHandler.GetSetting("UIScaling", 1.0);
        set => SettingsHandler.SetSetting("UIScaling", value.ToString());
    }

    [Setting("Performance", "Use Session Store", "Enables/Diables the session store", typeof(bool), isVisible: false)]
    public static bool UseSessionStore
    {
        get => SettingsHandler.GetSetting("UseSessionStore", false);
        set => SettingsHandler.SetSetting("UseSessionStore", value.ToString());
    }

    [Setting("Performance", "Max Backup Count", "Sets the maximum number of backups to keep (default: infinite)", typeof(int))]
    [Range(-1, 100)]
    public static int MaxBackupCount
    {
        get => SettingsHandler.GetSetting("MaxBackupCount", -1);
        set => SettingsHandler.SetSetting("MaxBackupCount", value.ToString());
    }

    [Setting("Performance", "Max Log Size", "Sets the max log size at which a warning appears", typeof(double))]
    [Range(-1.0, 100.0)]
    public static double MaxLogSize
    {
        get => SettingsHandler.GetSetting("MaxLogSize", -1.0);
        set => SettingsHandler.SetSetting("MaxLogSize", value.ToString());
    }

    [Setting("Database", "Show Extended Database Information", "Enables/Disables a context menu to show extended database information", typeof(bool))]
    public static bool ShowExtendedDatabaseInformation
    {
        get => SettingsHandler.GetSetting("ShowExtendedDatabaseInformation", false);
        set => SettingsHandler.SetSetting("ShowExtendedDatabaseInformation", value.ToString());
    }

    [Setting("Database", "Open Query On Server Added", "Enables/Disables the opening of a new query tab when a server is added", typeof(bool))]
    public static bool OpenQueryOnServerAdded
    {
        get => SettingsHandler.GetSetting("OpenQueryOnServerAdded", true);
        set => SettingsHandler.SetSetting("OpenQueryOnServerAdded", value.ToString());
    }

    [Setting("Startup", "Enable auto connect", "Enables the auto connect on startup function to a default server", typeof(bool))]
    public static bool AutoConnectEnabled
    {
        get => SettingsHandler.GetSetting("AutoConnectEnabled", true);
        set => SettingsHandler.SetSetting("AutoConnectEnabled", value.ToString());
    }

    [Setting("Startup", "Server name", "The server to which the program should automatically connect on startup", typeof(string))]
    [DependsUponSetting(nameof(AutoConnectEnabled))]
    public static string AutoConnectServerName
    {
        get => SettingsHandler.GetSetting("AutoConnectServerName", "LOCALHOST");
        set => SettingsHandler.SetSetting("AutoConnectServerName", value);
    }

    [Setting("Startup", "Use Integrated Security", "Using integrated security to auto connect on startup", typeof(bool))]
    [DependsUponSetting(nameof(AutoConnectEnabled))]
    public static bool AutoConnectIGS
    {
        get => SettingsHandler.GetSetting("AutoConnectIGS", true);
        set => SettingsHandler.SetSetting("AutoConnectIGS", value.ToString());
    }

    [Setting("Startup", "Username", "The username to auto connect on startup", typeof(string))]
    [DependsUponSetting(nameof(AutoConnectEnabled))]
    [DependsUponSetting(nameof(AutoConnectIGS), true)]
    public static string AutoConnectUsername
    {
        get => SettingsHandler.GetSetting("AutoConnectUsername", string.Empty);
        set => SettingsHandler.SetSetting("AutoConnectUsername", value);
    }

    [Setting("Startup", "Password", "The password to auto connect on startup", typeof(string))]
    [DependsUponSetting(nameof(AutoConnectEnabled))]
    [DependsUponSetting(nameof(AutoConnectIGS), true)]
    public static string AutoConnectPassword
    {
        get => SettingsHandler.GetSetting("AutoConnectPassword", string.Empty);
        set => SettingsHandler.SetSetting("AutoConnectPassword", value);
    }
  
    [Setting("Filtering", "Min filter chars", "How many characters should the filtering start with? Useful for huge databases/servers with many objects. Here the search can make sense from 3 characters. E.g. only \"e\" is quite pointless.", typeof(int))]
    [Range(1, 3)]
    public static int MinFilterChar
    {
        get => SettingsHandler.GetSetting("MinFilterChar", 3);
        set => SettingsHandler.SetSetting("MinFilterChar", value.ToString());
    }

    [Setting("Filtering", "Filter Update Rate (seconds)", "How often should the filtering be updated. Every x seconds. Attention, if the filter text is the same, the filtering is not executed again.", typeof(int))]
    [Range(1, 5)]
    public static int FilterUpdateRate
    {
        get => SettingsHandler.GetSetting("FilterUpdateRate", 2);
        set => SettingsHandler.SetSetting("FilterUpdateRate", value.ToString());
    }

    [Setting("Filtering", "Auto expand nodes", "Enables/Disables the automatic expanding of nodes when filtering", typeof(bool))]
    public static bool AutoExpandNodes
    {
        get => SettingsHandler.GetSetting("AutoExpandNodes", true);
        set => SettingsHandler.SetSetting("AutoExpandNodes", value.ToString());
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
