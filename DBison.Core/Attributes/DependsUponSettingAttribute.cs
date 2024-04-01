namespace DBison.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class DependsUponSettingAttribute(string dependingSetting, bool inverseBoolValue = false) : Attribute
{
    /// <summary>
    /// The depending setting.
    /// </summary>
    public string DependingSetting { get; } = dependingSetting;
    /// <summary>
    /// Determines if the bool value should be inversed.
    /// </summary>
    public bool InverseBoolValue { get; } = inverseBoolValue;
}
