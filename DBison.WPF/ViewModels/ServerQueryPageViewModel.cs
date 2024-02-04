using DBison.Core.Baseclasses;

namespace DBison.WPF.ViewModels;
public class ServerQueryPageViewModel : ViewModelBase
{
    public ServerQueryPageViewModel(string name)
    {
        Header = name;
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
}
