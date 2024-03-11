using DBison.Core.Entities;
using DBison.Core.Helper;
using DBison.WPF.ViewModels;

namespace DBison.WPF.HelperObjects;
public class QueryPageCreationReq
{
    public string Name { get; set; }
    public string QueryText { get; set; }
    public DatabaseObjectBase DataBaseObject { get; set; }
    public ExtendedDatabaseInfo ExtendedDatabaseRef { get; set; }
    public string SQL { get; set; }
    public ServerQueryHelper ServerQueryHelper { get; set; }
    public ServerViewModel ServerViewModel { get; set; }
}
