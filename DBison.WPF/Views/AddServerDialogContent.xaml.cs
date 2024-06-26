﻿using DBison.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBison.WPF.Views;
/// <summary>
/// Interaction logic for AddServerDialogContent.xaml
/// </summary>
public partial class AddServerDialogContent : UserControl
{
    public AddServerDialogContent()
    {
        InitializeComponent();
    }

    private void __PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AddServerDialogViewModel viewModel && sender is PasswordBox passwordBox)
            viewModel.Password = passwordBox.Password;
    }

    private void __Loaded(object sender, RoutedEventArgs e)
    {
        TBServerName.Focusable = true;
        Keyboard.Focus(TBServerName);
    }
}
