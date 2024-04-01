using DBison.Core.Entities.Enums;

namespace DBison.Core.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SettingAttribute(string groupName, string header, string tooltip, Type type, bool isVisible = true, eStringStyleVariation stringStyleVariation = eStringStyleVariation.Default) : Attribute
{
    /// <summary>
    /// The name of the setting group.
    /// </summary>
    public string GroupName { get; } = groupName;
    /// <summary>
    /// The header of the setting.
    /// </summary>
    public string Header { get; } = header;
    /// <summary>
    /// The tooltip of the setting.
    /// </summary>
    public string ToolTip { get; } = tooltip;
    /// <summary>
    /// Determines if the setting is visible.
    /// </summary>
    public bool IsVisible { get; } = isVisible;
    /// <summary>
    /// Determines which representation of the string style should be used. Possible options are: Default, Path, Password.
    /// </summary>
    public eStringStyleVariation StringStyleVariation { get; } = stringStyleVariation;
    /// <summary>
    /// The type of the setting that determines which style will be used.
    /// </summary>
    public Type Type { get; } = type;
}
