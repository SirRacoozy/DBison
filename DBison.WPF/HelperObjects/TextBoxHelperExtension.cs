using DBison.Core.Extender;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.HelperObjects;

public static class TextBoxHelperExtension
{
    #region [AutoClearTextButtonProperty]
    public static readonly DependencyProperty AutoClearTextButtonProperty =
        DependencyProperty.RegisterAttached(
            "AutoClearTextButton",
            typeof(bool),
            typeof(TextBoxHelperExtension),
            new FrameworkPropertyMetadata(false, __OnAutoClearTextButtonPropertyChanged));
    #endregion

    #region [GetAutoClearTextButton]
    public static bool GetAutoClearTextButton(DependencyObject obj)
    {
        return (bool)obj.GetValue(AutoClearTextButtonProperty);
    }
    #endregion

    #region [SetAutoClearTextButton]
    public static void SetAutoClearTextButton(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoClearTextButtonProperty, value);
    }
    #endregion

    #region [__OnAutoClearTextButtonPropertyChanged]
    private static void __OnAutoClearTextButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
            return;

        if ((bool)e.NewValue)
        {
            textBox.TextChanged += __TextBox_TextChanged;
            __UpdateClearButtonVisibility(textBox);
        }
        else
        {
            textBox.TextChanged -= __TextBox_TextChanged;
            TextBoxHelper.SetClearTextButton(textBox, false);
        }
    }
    #endregion

    #region [__TextBox_TextChanged]
    private static void __TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        __UpdateClearButtonVisibility(textBox);
    }
    #endregion

    #region [__UpdateClearButtonVisibility]
    private static void __UpdateClearButtonVisibility(TextBox textBox)
    {
        TextBoxHelper.SetClearTextButton(textBox, textBox.Text.IsNotNullOrEmpty());
    }
    #endregion
}