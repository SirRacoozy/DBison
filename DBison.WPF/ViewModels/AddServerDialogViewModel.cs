using DBison.Core.Attributes;
using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using DBison.Core.Extender;
using System.Windows;

namespace DBison.WPF.ViewModels;
public class AddServerDialogViewModel : ViewModelBase
{
    private Window m_Window;
    public event EventHandler<ServerInfo> OkClicked;

    public AddServerDialogViewModel(Window window)
    {
        m_Window = window;
        ServerName = Environment.MachineName;
    }

    [DependsUpon(nameof(IntegratedSecurity))]
    public Visibility CredentialsVisibility => IntegratedSecurity ? Visibility.Collapsed : Visibility.Visible;

    public bool IntegratedSecurity
    {
        get => Get<bool>();
        set => Set(value);
    }

    public string ServerName
    {
        get => Get<string>();
        set => Set(value);
    }

    public string UserName
    {
        get => Get<string>();
        set => Set(value);
    }

    public string Password
    {
        get => Get<string>();
        set => Set(value);
    }

    [DependsUpon(nameof(ServerName))]
    [DependsUpon(nameof(IntegratedSecurity))]
    [DependsUpon(nameof(UserName))]
    [DependsUpon(nameof(Password))]
    public bool CanExecute_Ok()
    {
        if (ServerName.IsNullOrEmpty())
            return false;

        if (IntegratedSecurity)
        {
            return true;
        }
        else
        {
            return UserName.IsNotNullOrEmpty() && Password.IsNotNullOrEmpty();
        }

    }

    public void Execute_Ok()
    {
        var server = new ServerInfo(ServerName)
        {
            UseIntegratedSecurity = IntegratedSecurity,
            Username = UserName,
            Password = Password,
        };
        OkClicked?.Invoke(this, server);
        m_Window.Close();
    }

    public void Execute_Cancel()
    {
        m_Window.Close();
    }

}
