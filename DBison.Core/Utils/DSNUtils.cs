using DBison.Core.Entities;
using DBison.Core.Entities.Enums;
using DBison.Core.Extender;
using DBison.Core.Utils.SettingsSystem;
using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace DBison.Core.Utils;

[SupportedOSPlatform("windows")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "DSN is a standing acronym and therefore should be capitalized.")]
public static class DSNUtils
{
#pragma warning disable IDE1006 // Naming Styles
    private const string USER_SUB_KEY = @"SOFTWARE\ODBC\ODBC.INI";
    private const string SYSTEM_X86_KEY = @"SOFTWARE\WOW6432Node\ODBC\ODBC.INI";
    private const string SYSTEM_X64_KEY = @"SOFTWARE\ODBC\ODBC.INI";
    private static readonly string m_MSSQLDriver = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\ODBC\ODBCINST.INI\SQL Server")?.GetValue("Driver")?.ToStringValue() ?? string.Empty;
#pragma warning restore IDE1006 // Naming Styles

    public static List<DSNEntry> GetAllDSNEntries()
    {
        List<DSNEntry> dsnEntries = new();

        dsnEntries.AddRange(__GetDSNEntryForArchitectureAndLocation(true, eDSNArchitecture.x86));
        dsnEntries.AddRange(__GetDSNEntryForArchitectureAndLocation(true, eDSNArchitecture.x64));
        dsnEntries.AddRange(__GetDSNEntryForArchitectureAndLocation(false));

        return dsnEntries;
    }

    public static void SetDSNEntry(DSNEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var keys = __OpenRegistryKeys();
        if(keys is not null)
        {
            foreach (var key in keys)
                __WriteSingleDSNEntry(key, entry);
        }
    }

    private static void __WriteSingleDSNEntry(RegistryKey key, DSNEntry entry)
    {
        var dsnName = entry.DSNPattern.Replace("{{ServerName}}", entry.ServerName).Replace("{{DataBase}}", entry.DatabaseName).Replace("{{DateTimeNow}}", DateTime.Now.ToString("ddMMyyyy_HHmmss"));
        if (dsnName.Length > 32)
            dsnName = dsnName.Substring(0, 32);

        var subKey = key.CreateSubKey(dsnName);
        subKey.SetValue("Server", entry.ServerName);
        subKey.SetValue("Database", entry.DatabaseName);
        subKey.SetValue("LastUser", entry.Username);
        subKey.SetValue("Driver", m_MSSQLDriver);

        if (entry.TrustedConnection)
            subKey.SetValue("Trusted_Connection", "Yes");

        subKey.Close();
        key.Close();
    }

    private static List<DSNEntry> __GetDSNEntryForArchitectureAndLocation(bool useSystemMode, eDSNArchitecture architecture = eDSNArchitecture.x86x64)
    {
        List<DSNEntry> dsnEntries = new();
        RegistryKey? key = null;
        if(useSystemMode)
        {
            if (architecture == eDSNArchitecture.x64 || architecture == eDSNArchitecture.x86x64)
                key = Registry.LocalMachine.OpenSubKey(SYSTEM_X64_KEY);
            else if (architecture == eDSNArchitecture.x86 || architecture == eDSNArchitecture.x86x64)
                key = Registry.LocalMachine.OpenSubKey(SYSTEM_X86_KEY);
        }
        else
        {
            key = Registry.CurrentUser.OpenSubKey(USER_SUB_KEY);
        }

        if (key == null)
            return dsnEntries;

        foreach(var keyName in key.GetSubKeyNames())
        {
            var subKey = key.OpenSubKey(keyName);

            if(subKey != null)
            {
                DSNEntry entry = new
                (
                    DSNPattern: subKey.Name,
                    DatabaseName: subKey.GetValue("Database")?.ToString() ?? string.Empty,
                    Username: subKey.GetValue("LastUser")?.ToString() ?? string.Empty,
                    ServerName: subKey.GetValue("Server")?.ToString() ?? string.Empty,
                    TrustedConnection: false,
                    Architecture: architecture
                );

                if(subKey.GetValueNames().Contains("Trusted_Connection"))
                {
                    entry = entry with
                    {
                        TrustedConnection = true
                    };
                }

                dsnEntries.Add(entry);
            }
        }

        return dsnEntries;
    }

    private static List<RegistryKey> __OpenRegistryKeys()
    {
        List<RegistryKey> keys = new();
        if (Settings.UseSystemDSN)
        {
            var dsnArchitecture = Settings.DSNArchitecture;
            if (dsnArchitecture == eDSNArchitecture.x64 || dsnArchitecture == eDSNArchitecture.x86x64)
            {
                var key = Registry.LocalMachine.OpenSubKey(SYSTEM_X64_KEY, true);
                if (key is not null)
                    keys.Add(key);
            }
            else if (dsnArchitecture == eDSNArchitecture.x86 || dsnArchitecture == eDSNArchitecture.x86x64)
            {
                var key = Registry.LocalMachine.OpenSubKey(SYSTEM_X86_KEY, true);
                if (key is not null)
                    keys.Add(key);
            }
        }
        else
        {
            var key = Registry.CurrentUser.OpenSubKey(USER_SUB_KEY, true);
            if (key is not null)
                keys.Add(key);
        }
        return keys;
    }
}
