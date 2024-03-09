namespace DBison.Core.EventArguments;

public class SettingChangedEventArgs(string changedSettingName) : EventArgs
{
    public string ChangedSettingName { get; } = changedSettingName;
}
