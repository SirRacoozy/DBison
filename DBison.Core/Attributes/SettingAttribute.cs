using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.Core.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SettingAttribute(string groupName, string header, Type type, bool isVisible = true) : Attribute
{
    public string GroupName { get; } = groupName;
    public string Header { get; } = header;
    public bool IsVisible { get; } = isVisible;
    public Type Type { get; } = type;
}
