using DBison.WPF.ClientBaseClasses;
using System.Data;

namespace DBison.WPF.ViewModels;
public class ResultSetViewModel : ClientViewModelBase
{
    #region - ctor -
    public ResultSetViewModel()
    {
    }
    #endregion

    #region [ResultLines]
    public DataView ResultLines
    {
        get => Get<DataView>();
        set => Set(value);
    }
    #endregion
}
