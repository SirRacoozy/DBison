﻿using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls
{
    public class DBisonTextBox : TextBox
    {
        public DBisonTextBox()
        {
            TextChanged += __DBisonTextBox_TextChanged;
        }
        static DBisonTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DBisonTextBox), new FrameworkPropertyMetadata(typeof(DBisonTextBox)));
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DBisonTextBox));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


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
    }
}