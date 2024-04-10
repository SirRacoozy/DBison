using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;

namespace DBison.WPF.ViewModels;
public class DataBaseObjectCache
{
    private List<DatabaseObjectBase> m_Objects = new List<DatabaseObjectBase>();
    private ServerQueryHelper m_Helper;

    private DatabaseInfo m_DatabaseInfo;

    #region [DataBaseObjectCache]
    public DataBaseObjectCache(DatabaseInfo database, Core.Helper.ServerQueryHelper serverQueryHelper)
    {
        m_DatabaseInfo = database;
        m_Helper = serverQueryHelper;
        __LoadObjects();
    }
    #endregion

    #region [Reset]
    public void Reset()
    {
        m_Objects.Clear();
    }
    #endregion

    #region [GetObjectsWithContains]
    public List<DatabaseObjectBase> GetObjectsWithContains(string like)
    {
        if (m_Objects.IsEmpty())
            __LoadObjects();
        if (like.IsNullOrEmpty())
            return m_Objects;
        return new(m_Objects.Where(o => o.Name.Contains(like, StringComparison.InvariantCultureIgnoreCase)));
    }
    #endregion

    #region [__LoadObjects]
    private void __LoadObjects()
    {
        m_Objects = m_Helper.GetAllObjects(m_DatabaseInfo);
    }
    #endregion
}
