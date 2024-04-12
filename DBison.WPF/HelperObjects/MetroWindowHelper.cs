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
        btn.Width = 25;
        btn.MouseEnter += (sender, e) => __ChangeTitleButtonBorderContentVisibility(btn, Visibility.Visible);
        btn.MouseLeave += (sender, e) => __ChangeTitleButtonBorderContentVisibility(btn, Visibility.Hidden);
    }
    #endregion

    #region [__GetButtonInitContent]
    private static UIElement __GetButtonInitContent(string baseContentResourceName)
    {
        if (Application.Current.Resources[baseContentResourceName] is UIElement Content)
        {
            __ChangeTitleButtonBorderContentVisibility(Content, Visibility.Hidden);
            return Content;
        }
        return null;
    }
    #endregion

    #region [__ChangeTitleButtonBorderContentVisibility]
    private static void __ChangeTitleButtonBorderContentVisibility(UIElement content, Visibility visibility)
    {
        if (content == null)
            return;
        var border = content.FindChild<Border>("ButtonBorder");
        if (border == null)
            return;
        if (visibility == Visibility.Visible)
            border.Opacity = 1;
        else
            border.Opacity = 0.2;
        if (border.Child is UIElement uiElem)
        {
            uiElem.Visibility = visibility;
        }
    }
    #endregion

}