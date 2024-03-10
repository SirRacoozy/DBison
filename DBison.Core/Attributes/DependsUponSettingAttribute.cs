namespace DBison.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class DependsUponSettingAttribute(string dependingSetting, bool inverseBoolValue = false) : Attribute
{
    public string DependingSetting { get; } = dependingSetting;
    public bool InverseBoolValue { get; } = inverseBoolValue;
}
