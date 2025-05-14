using DBison.Core.Extender;
using DBison.Core.Utils.SettingsSystem;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for MultiplePanelControl.xaml
/// </summary>
public partial class MultiplePanelControl : UserControl
{
    private Dictionary<CrossButton, object> m_RemoveRefs = new Dictionary<CrossButton, object>();
    public MultiplePanelControl()
    {
        InitializeComponent();
    }

    #region [CanClose]
    public bool CanClose
    {
        get { return (bool)GetValue(CanCloseProperty); }
        set { SetValue(CanCloseProperty, value); }
    }

    public static readonly DependencyProperty CanCloseProperty =
        DependencyProperty.Register("CanClose", typeof(bool), typeof(MultiplePanelControl), new PropertyMetadata(false));
    #endregion

    #region [ReadyForBuildControls]
    public bool ReadyForBuildControls
    {
        get { return (bool)GetValue(ReadyForBuildControlsProperty); }
        set { SetValue(ReadyForBuildControlsProperty, value); }
    }

    public static readonly DependencyProperty ReadyForBuildControlsProperty =
        DependencyProperty.Register("ReadyForBuildControls", typeof(bool), typeof(MultiplePanelControl), new PropertyMetadata(true));
    #endregion

    #region [RebuildNeeded]
    public bool RebuildNeeded
    {
        get { return (bool)GetValue(RebuildNeededProperty); }
        set { SetValue(RebuildNeededProperty, value); }
    }

    public static readonly DependencyProperty RebuildNeededProperty =
        DependencyProperty.Register("RebuildNeeded", typeof(bool), typeof(MultiplePanelControl), new PropertyMetadata(false, __RebuildNeededChanged));

    #endregion

    #region [Content]
    public object Content
    {
        get { return (object)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(object), typeof(MultiplePanelControl));
    #endregion

    #region [ItemsSource]

    public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register(
        "ItemsSource", typeof(IEnumerable<object>), typeof(MultiplePanelControl),
        new PropertyMetadata(null, __ItemsSourcePropertyChanged));

    public IEnumerable<object> ItemsSource
    {
        get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    private static void __ItemsSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiplePanelControl)obj;

        if (e.OldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -= control.__ItemsSourceCollectionChanged;
        }

        if (e.NewValue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged += control.__ItemsSourceCollectionChanged;
        }

        control.__UpdateItemsSource();
    }

    private void __ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        try
        {
            __UpdateItemsSource();
        }
        catch (Exception ex)
        {
        }
    }

    private void __UpdateItemsSource()
    {
        if (!ReadyForBuildControls)
            return;
        Content = null;
        if (ItemsSource.IsEmpty())
        {
            return;
        }
        else
        {
            Content = new TextBlock { Text = $"Itemssource of {ItemsSource.Count()}", FontSize = 40, Foreground = Brushes.Green, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            __GenerateControlContent();
        }
    }
    #endregion

    #region [__GenerateControlContent]
    private void __GenerateControlContent()
    {
        var mainGrid = new Grid();
        mainGrid.Loaded += __MainGrid_Loaded;
        Content = mainGrid;
    }
    #endregion

    private void __MainGrid_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is Grid mainGrid)
            __Rebuild(mainGrid);
    }

    private void __Rebuild(Grid mainGrid)
    {
        if (ItemsSource == null)
            return;

        mainGrid.Children.Clear();
        mainGrid.RowDefinitions.Clear();
        m_RemoveRefs.Clear();

        int itemsSourceCount = ItemsSource.Count();
        int neededRows = itemsSourceCount + (itemsSourceCount > 1 ? itemsSourceCount + 1 : 0);
        int currentContentItem = 0;
        int gridSplitterHeight = 5;

        for (int i = 0; i < neededRows; i++)
        {
            var rowDefinition = new RowDefinition
            {
                Height = (i % 2 == 0)
                    ? new GridLength(1, GridUnitType.Star)
                    : new GridLength(gridSplitterHeight)
            };
            mainGrid.RowDefinitions.Add(rowDefinition);
        }

        for (int i = 0; i < neededRows; i++)
        {
            if (i % 2 == 0)
            {
                if (ItemsSource.Count() > currentContentItem)
                {
                    var item = ItemsSource.ElementAt(currentContentItem++);
                    if (item is FrameworkElement elem)
                    {
                        try
                        {
                            if (elem.Parent is Panel p) p.Children.Remove(elem);
                            else if (elem.Parent is ContentControl cc) cc.Content = null;
                            else if (elem.Parent is Decorator d) d.Child = null;

                            var border = new Border
                            {
                                BorderThickness = new Thickness(1),
                                BorderBrush = Settings.UseDarkMode ? Brushes.White : Brushes.Black,
                                Margin = new Thickness(0, 5, 0, 5),
                            };

                            if (CanClose)
                            {
                                var crossButton = new CrossButton
                                {
                                    Width = 20,
                                    Height = 20,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness(5)
                                };

                                m_RemoveRefs[crossButton] = item;
                                crossButton.Click += __CrossButton_Click;

                                var horizontalStack = new StackPanel
                                {
                                    Orientation = Orientation.Horizontal
                                };
                                horizontalStack.Children.Add(crossButton);
                                horizontalStack.Children.Add(elem);

                                border.Child = horizontalStack;
                            }
                            else
                            {
                                border.Child = elem;
                            }

                            mainGrid.Children.Add(border);
                            Grid.SetRow(border, i);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[Rebuild Error] {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                try
                {
                    var splitter = new GridSplitter
                    {
                        Height = gridSplitterHeight,
                        Background = Settings.UseDarkMode ? Brushes.White : Brushes.Black,
                        ResizeDirection = GridResizeDirection.Rows,
                    };

                    mainGrid.Children.Add(splitter);
                    Grid.SetRow(splitter, i);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Splitter Error] {ex.Message}");
                }
            }
        }
    }


    private static void __RebuildNeededChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiplePanelControl)obj;
        if (e.NewValue is bool boolean && boolean)
        {
            control.RebuildNeeded = false;
            control.__UpdateItemsSource();
        }
    }

    private void __CrossButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is CrossButton cb && Content is Grid grd)
        {
            if (m_RemoveRefs.TryGetValue(cb, out object itemToRemove) && ItemsSource is IList lst && ItemsSource.Contains(itemToRemove))
            {
                if (itemToRemove != null)
                {
                    lst.Remove(itemToRemove);
                }
            }
        }
    }
}