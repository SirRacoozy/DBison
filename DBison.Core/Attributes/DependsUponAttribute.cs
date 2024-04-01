namespace DBison.Core.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
public class DependsUponAttribute : Attribute
{
    /// <summary>
    /// The member name that the property depends upon. 
    /// </summary>
    public string MemberName;

    /// <summary>
    /// Creates a DependsUpon attribute.
    /// </summary>
    /// <param name="memberName">The member name that this property depends upon.</param>
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
