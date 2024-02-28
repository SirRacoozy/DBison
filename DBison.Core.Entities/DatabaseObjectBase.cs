namespace DBison.Core.Entities;
public abstract class DatabaseObjectBase
{
    public DatabaseObjectBase(string name)
    {
        Name = name;
    }

    #region [Name]
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    #endregion

    public bool IsMainNode { get; set; }
}
