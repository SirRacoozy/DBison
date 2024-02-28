namespace DBison.Core.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
public class DependsUponAttribute : Attribute
{
    public string MemberName;
    public DependsUponAttribute(string memberName)
    {
        MemberName = memberName;
    }
}

public class DependsUponObject
{
    public DependsUponObject()
    {
        DependendObjects = new List<string>();
    }
    public List<string> DependendObjects { get; set; }
}
