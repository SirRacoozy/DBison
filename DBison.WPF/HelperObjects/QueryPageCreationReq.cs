using DBison.Core.Entities;
using DBison.Core.Helper;
using DBison.WPF.ViewModels;

namespace DBison.WPF.HelperObjects;
public class QueryPageCreationReq
{
    #region [Name]
    public string Name { get; set; }
    #endregion

    #region [QueryText]
    public string QueryText { get; set; }
    #endregion

    #region [DataBaseObject]
    public DatabaseObjectBase DataBaseObject { get; set; }
    #endregion

    #region [ExtendedDatabaseRef]
    public ExtendedDatabaseInfo ExtendedDatabaseRef { get; set; }
    #endregion

    #region [SQL]
    public string SQL { get; set; }
    #endregion

    #region [ServerQueryHelper]
    public ServerQueryHelper ServerQueryHelper { get; set; }
    #endregion

    #region [ServerViewModel]
    public ServerViewModel ServerViewModel { get; set; }
    #endregion
}
