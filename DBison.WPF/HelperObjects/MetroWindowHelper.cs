using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.HelperObjects;
internal static class MetroWindowHelper
{
    #region [SetButtons]
    internal static void SetButtons(this MetroWindow window)
    {
        var WindowButtonCommands = window.WindowButtonCommands;
        if (WindowButtonCommands.Template.FindName("PART_Min", WindowButtonCommands) is Button minButton)
        {
            minButton.Content = __GetButtonInitContent("MinButtonContent");
            minButton.Style = Application.Current.Resources["TitleBarBtnStyle"] as Style;
            __ButtonEvents(minButton);
        }

        if (WindowButtonCommands.Template.FindName("PART_Max", WindowButtonCommands) is Button maxButton)
        {
            maxButton.Content = __GetButtonInitContent("MaxButtonContent");
            maxButton.Style = Application.Current.Resources["TitleBarBtnStyle"] as Style;
            __ButtonEvents(maxButton);
        }

        if (WindowButtonCommands.Template.FindName("PART_Close", WindowButtonCommands) is Button closeButton)
        {
            closeButton.Content = __GetButtonInitContent("CloseButtonContent");
            closeButton.Style = Application.Current.Resources["TitleBarBtnStyle"] as Style;
            __ButtonEvents(closeButton);
        }
    }
    #endregion

    #region [__ButtonEvents]
    private static void __ButtonEvents(Button btn)
    {
        var width = btn.Width;
        btn.Width = 25;
        btn.MouseEnter += (sender, e) =>
        {
            var border = btn.FindChild<Border>("ButtonBorder");
            if (border != null && border.Child is UIElement uiElem)
                uiElem.Visibility = Visibility.Visible;
        };
        btn.MouseLeave += (sender, e) => __ClearButtonContent(btn);
    }
    #endregion

    #region [__ClearButtonContent]
    private static void __ClearButtonContent(Button btn)
    {
        var border = btn.FindChild<Border>("ButtonBorder");
        if (border != null && border.Child is UIElement uiElem)
            uiElem.Visibility = Visibility.Hidden;
    }
    #endregion

    #region [__GetButtonInitContent]
    private static UIElement __GetButtonInitContent(string baseContentResourceName)
    {
        var Content = Application.Current.Resources[baseContentResourceName] as UIElement;
        var border = Content.FindChild<Border>("ButtonBorder");
        if (border != null && border.Child is UIElement uiElem)
            uiElem.Visibility = Visibility.Hidden;
        return Content;
    }
    #endregion
}