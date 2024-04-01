using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for WindowChromedContent.xaml
/// </summary>
public partial class WindowChromedContent : UserControl
{
    public static readonly DependencyProperty WindowProperty =
        DependencyProperty.Register("Window", typeof(Window), typeof(WindowChromedContent), new PropertyMetadata(null, __WindowChanged));

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(UIElement), typeof(WindowChromedContent), new PropertyMetadata(null));

    #region [WindowChromedContent]
    public WindowChromedContent()
    {
        InitializeComponent();
    }
    #endregion

    #region [Window]
    public Window Window
    {
        get { return (Window)GetValue(WindowProperty); }
        set { SetValue(WindowProperty, value); }
    }
    #endregion

    #region [Content]
    public UIElement Content
    {
        get { return (UIElement)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }
    #endregion

    #region [__WindowChanged]
    private static void __WindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null && e.NewValue is Window wnd)
        {
            var xThis = ((WindowChromedContent)d);
            if (xThis == null)
                return;
            wnd.StateChanged += xThis.__MainWindowStateChangeRaised;
            xThis.__AddCommandBindings();
        }
    }
    #endregion

    #region [__AddCommandBindings]
    private void __AddCommandBindings()
    {
        if (Window == null)
            return;

        Window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, __Execute_Close, __CanExecute));
        Window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, __Execute_Maximize, __CanExecute));
        Window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, __Execute_Minimize, __CanExecute));
        Window.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, __Execute_Restore, __CanExecute));
    }
    #endregion

    #region [__MouseLeftButtonDown]
    private void __MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (Window == null)
            return;
        if (e.ClickCount == 2)
        {
            if (Window.WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(Window);
            else
                SystemCommands.MaximizeWindow(Window);
        }
    }
    #endregion

    #region [__MainWindowStateChangeRaised]
    private void __MainWindowStateChangeRaised(object sender, EventArgs e)
    {
        if (Window == null)
            return;
        if (Window.WindowState == WindowState.Maximized)
        {
            RestoreButton.Visibility = Visibility.Visible;
            MaximizeButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            RestoreButton.Visibility = Visibility.Collapsed;
            MaximizeButton.Visibility = Visibility.Visible;
        }
    }
    #endregion

    #region [__CanExecute]
    private void __CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }
    #endregion

    #region [__Execute_Minimize]
    private void __Execute_Minimize(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(Window);
    }
    #endregion

    #region [__Execute_Maximize]
    private void __Execute_Maximize(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MaximizeWindow(Window);
    }
    #endregion

    #region [__Execute_Restore]
    private void __Execute_Restore(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.RestoreWindow(Window);
    }
    #endregion

    #region [__Execute_Close]
    private void __Execute_Close(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.CloseWindow(Window);
    }
    #endregion
}
