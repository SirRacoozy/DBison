using DBison.WPF.ClientBaseClasses;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for KeyGridSplitter.xaml
/// </summary>
public partial class KeyGridSplitter : UserControl
{
    public KeyGridSplitter()
    {
        PreviewKeyDown += __KeyGridSplitter_KeyDown;
        InitializeComponent();
    }

    #region ContentPanel1
    public object ContentPanel1
    {
        get { return (object)GetValue(ContentPanel1Property); }
        set { SetValue(ContentPanel1Property, value); }
    }

    public static readonly DependencyProperty ContentPanel1Property =
            DependencyProperty.RegisterAttached("ContentPanel1", typeof(object), typeof(KeyGridSplitter), new PropertyMetadata(null));
    #endregion

    #region ContentPanel2
    public object ContentPanel2
    {
        get { return (object)GetValue(ContentPanel2Property); }
        set { SetValue(ContentPanel2Property, value); }
    }

    public static readonly DependencyProperty ContentPanel2Property =
            DependencyProperty.RegisterAttached("ContentPanel2", typeof(object), typeof(KeyGridSplitter), new PropertyMetadata(null));
    #endregion

    #region ResizeDirection
    public Orientation ResizeDirection
    {
        get { return (Orientation)GetValue(ResizeDirectionProperty); }
        set { SetValue(ResizeDirectionProperty, value); }
    }

    public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.RegisterAttached("ResizeDirection", typeof(Orientation), typeof(KeyGridSplitter), new PropertyMetadata(Orientation.Vertical));

    #endregion

    #region [__MouseWheelClick]
    private void __MouseWheelClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle && sender is Grid grd && grd.DataContext is TabItemViewModelBase tabItemBase)
        {
            tabItemBase.Execute_Close();
        }
    }
    #endregion

    #region [__KeyGridSplitter_KeyDown]
    private void __KeyGridSplitter_KeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt))
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Up:
                    __MoveSplitter(-10);
                    e.Handled = true;
                    break;
                case Key.Right:
                case Key.Down:
                    __MoveSplitter(10);
                    e.Handled = true;
                    break;
            }
        }
    }
    #endregion

    #region [__MoveSplitter]
    private void __MoveSplitter(int direction)
    {
        var grid = this.FindChild<Grid>("PanelsGrid");
        if (grid == null) return;

        if (ResizeDirection == Orientation.Horizontal)
        {
            var firstColumn = grid.ColumnDefinitions.FirstOrDefault();
            if (firstColumn != null)
            {
                double newWidth = firstColumn.ActualWidth;
                if (firstColumn.Width.IsStar)
                    newWidth = firstColumn.ActualWidth;
                newWidth += Convert.ToDouble(direction);
                if (newWidth < 10)
                    newWidth = 10;
                firstColumn.Width = new GridLength(newWidth);
            }
        }
        else
        {
            var firstRow = grid.RowDefinitions.FirstOrDefault();
            if (firstRow != null)
            {
                double newHeight = firstRow.ActualHeight;
                if (firstRow.Height.IsStar)
                    newHeight = firstRow.ActualHeight;
                newHeight += Convert.ToDouble(direction);
                if (newHeight < 10)
                    newHeight = 10;
                firstRow.Height = new GridLength(newHeight);
            }
        }
    }
    #endregion

}