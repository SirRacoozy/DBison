namespace DBison.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SettingAttribute(string groupName, string header, string tooltip, Type type, bool isVisible = true) : Attribute
{
    public string GroupName { get; } = groupName;
    public string Header { get; } = header;
    public string ToolTip { get; } = tooltip;
    public bool IsVisible { get; } = isVisible;
    public Type Type { get; } = type;
}
