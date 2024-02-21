using DBison.WPF.ClientBaseClasses;
using System.Data;

namespace DBison.WPF.ViewModels;
public class ResultSetViewModel : ClientViewModelBase
{
    public ResultSetViewModel()
    {
    }

    public DataView ResultLines
    {
        get => Get<DataView>();
        set => Set(value);
    }
}
