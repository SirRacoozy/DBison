using DBison.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for SettingsTreeView.xaml
/// </summary>
public partial class SettingsTreeView : UserControl
{
    #region [SettingsTreeView]
    public SettingsTreeView()
    {
        Loaded += __SettingsTreeView_Loaded;
        InitializeComponent();
    }

    #endregion

    #region [__SettingsTreeView_Loaded]
    private void __SettingsTreeView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsTabViewModel vm)
            vm.TreeView = ThisTreeView;
    }
    #endregion

    #region [__SelectedItemChanged]
    private void __SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is TreeView tv && tv.DataContext is SettingsTabViewModel settingsVm && tv.SelectedItem is SettingGroupViewModel groupVm)
            settingsVm.SelectedSettingsGroup = groupVm;
    }
    #endregion
}
