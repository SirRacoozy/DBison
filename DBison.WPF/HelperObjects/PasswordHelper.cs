using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.HelperObjects
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password",
        typeof(string), typeof(PasswordHelper),
        new FrameworkPropertyMetadata(string.Empty, __OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, __Attach));

        private static readonly DependencyProperty m_IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PasswordHelper));

        #region [SetAttach]
        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }
        #endregion

        #region [GetAttach]
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }
        #endregion

        #region [GetPassword]
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }
        #endregion

        #region [SetPassword]
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }
        #endregion

        #region [__GetIsUpdating]
        private static bool __GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(m_IsUpdatingProperty);
        }
        #endregion

        #region [__SetIsUpdating]
        private static void __SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(m_IsUpdatingProperty, value);
        }
        #endregion

        #region [__OnPasswordPropertyChanged]
        private static void __OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= __PasswordChanged;

            if (!(bool)__GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += __PasswordChanged;
        }
        #endregion

        #region [__Attach]
        private static void __Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= __PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += __PasswordChanged;
            }
        }
        #endregion

        #region [__PasswordChanged]
        private static void __PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            __SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            __SetIsUpdating(passwordBox, false);
        }
        #endregion
    }
}
