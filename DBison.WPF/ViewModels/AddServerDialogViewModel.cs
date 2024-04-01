using DBison.Core.Attributes;
using DBison.Core.Extender;
using DBison.Core.PluginSystem;
using DBison.Core.Utils.SettingsSystem;
using DBison.Plugin.Entities;
using DBison.WPF.ClientBaseClasses;
using System.Windows;

namespace DBison.WPF.ViewModels;
public class AddServerDialogViewModel : ClientViewModelBase
{
    private Window m_Window;
    public event EventHandler<ConnectInfo> OkClicked;

    public AddServerDialogViewModel(Window window)
    {
        m_Window = window;
        ServerName = Environment.MachineName;
        IntegratedSecurity = Settings.AutoConnectIGS;
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
        var pluginLoader = PluginLoader.Instance;
        var connectPlugin = pluginLoader.ConnectParsingPlugins.FirstOrDefault();

        ConnectInfo? pluginResult = null;

        if (connectPlugin != null)
            pluginResult = connectPlugin.ParseConnectInput(ServerName);

        var connectInfo = new ConnectInfo()
        {
            ServerName = ServerName,
            DatabaseName = string.Empty,
            UseIntegratedSecurity = IntegratedSecurity,
            Username = IntegratedSecurity ? Environment.UserName : UserName,
            Password = Password,
        };
        OkClicked?.Invoke(this, pluginResult ?? connectInfo);
        m_Window.Close();
    }

    public void Execute_Cancel()
    {
        m_Window.Close();
    }

}
