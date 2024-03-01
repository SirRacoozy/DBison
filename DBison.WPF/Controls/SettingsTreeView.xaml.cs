using DBison.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for SettingsTreeView.xaml
/// </summary>
public partial class SettingsTreeView : UserControl
{
    public SettingsTreeView()
    {
        InitializeComponent();
    }

    public TreeView TreeView => ThisTreeView;

    private void __SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is TreeView tv && tv.DataContext is SettingsViewModel settingsVm && tv.SelectedItem is SettingGroupViewModel groupVm)
            settingsVm.SelectedSettingsGroup = groupVm;
    }
}
