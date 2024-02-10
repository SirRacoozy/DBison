using DBison.Core.Attributes;
using DBison.Core.Baseclasses;
using DBison.Core.Extender;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace DBison.WPF.ViewModels;
public class ServerQueryPageViewModel : ViewModelBase
{
    ServerViewModel m_ServerViewModel;
    public ServerQueryPageViewModel(string name, ServerViewModel serverViewModel)
    {
        m_ServerViewModel = serverViewModel;
        Header = name;
        ResultSets = new ObservableCollection<ResultSetViewModel>();
    }

    public string Header
    {
        get => Get<string>();
        set => Set(value);
    }

    public string QueryText
    {
        get => Get<string>();
        set => Set(value);
    }

    public bool ResultOnly
    {
        get => Get<bool>();
        set => Set(value);
    }

    public ObservableCollection<ResultSetViewModel> ResultSets
    {
        get => Get<ObservableCollection<ResultSetViewModel>>();
        set => Set(value);
    }

    [DependsUpon(nameof(ResultOnly))]
    public Visibility QueryVisibility => ResultOnly ? Visibility.Collapsed : Visibility.Visible;

    public void Execute_Close()
    {
        m_ServerViewModel.RemoveQuery(this);
    }

    public void Execute_ExecuteSQL()
    {
      
    }

}
