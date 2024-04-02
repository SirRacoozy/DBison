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

    #region - ctor -
    public AddServerDialogViewModel(Window window)
    {
        m_Window = window;
        ServerName = Environment.MachineName;
        IntegratedSecurity = Settings.AutoConnectIGS;
    }
    #endregion

    #region [CredentialsVisibility]
    [DependsUpon(nameof(IntegratedSecurity))]
    public Visibility CredentialsVisibility => IntegratedSecurity ? Visibility.Collapsed : Visibility.Visible;
    #endregion

    #region [IntegratedSecurity]
    public bool IntegratedSecurity
    {
        get => Get<bool>();
        set => Set(value);
    }
    #endregion

    #region [ServerName]
    public string ServerName
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [UserName]
    public string UserName
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [Password]
    public string Password
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [CanExecute_Ok]
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
    #endregion

    #region [Execute_Ok]
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
        m_Window?.Close();
    }
    #endregion

    #region [Execute_Cancel]
    public void Execute_Cancel()
    {
        m_Window.Close();
    }
    #endregion

    public event EventHandler<ConnectInfo> OkClicked;
}
