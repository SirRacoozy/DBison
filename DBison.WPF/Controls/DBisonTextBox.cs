using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls
{
    public class __DBisonTextBox : TextBox
    {
        #region - needs -
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(__DBisonTextBox)); 
        #endregion

        #region [__DBisonTextBox]
        public __DBisonTextBox()
        {
            TextChanged += __DBisonTextBox_TextChanged;
        }
        #endregion

        #region [__DBisonTextBox]
        static __DBisonTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(__DBisonTextBox), new FrameworkPropertyMetadata(typeof(__DBisonTextBox)));
        }
        #endregion

        #region [CornerRadius]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion

        #region [__DBisonTextBox_TextChanged]
        private void __DBisonTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (tb.Text.Length > 0)
                    TextBoxHelper.SetClearTextButton(tb, true);
                else
                    TextBoxHelper.SetClearTextButton(tb, false);
            }
        }
        #endregion
    }
}
