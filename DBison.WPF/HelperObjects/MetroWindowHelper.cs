using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.HelperObjects;
internal static class MetroWindowHelper
{
    internal static void SetButtons(this MetroWindow window)
    {
        var WindowButtonCommands = window.WindowButtonCommands;
        if (WindowButtonCommands.Template.FindName("PART_Min", WindowButtonCommands) is Button minButton)
        {
            minButton.Content = System.Windows.Application.Current.Resources["MinButtonContent"];
            minButton.Style = System.Windows.Application.Current.Resources["TitleBarBtnStyle"] as Style;
        }

        if (WindowButtonCommands.Template.FindName("PART_Max", WindowButtonCommands) is Button maxButton)
        {
            maxButton.Content = System.Windows.Application.Current.Resources["MaxButtonContent"];
            maxButton.Style = System.Windows.Application.Current.Resources["TitleBarBtnStyle"] as Style;
        }

        if (WindowButtonCommands.Template.FindName("PART_Close", WindowButtonCommands) is Button closeButton)
        {
            closeButton.Content = System.Windows.Application.Current.Resources["CloseButtonContent"];
            closeButton.Style = System.Windows.Application.Current.Resources["TitleBarBtnStyle"] as Style;
        }
    }
}