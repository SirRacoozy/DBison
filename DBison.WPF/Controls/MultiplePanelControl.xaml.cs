using DBison.Core.Extender;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DBison.WPF.Controls;
/// <summary>
/// Interaction logic for MultiplePanelControl.xaml
/// </summary>
public partial class MultiplePanelControl : UserControl
{
    public MultiplePanelControl()
    {
        InitializeComponent();
    }

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
        var mainGrid = sender as Grid;
        var itemsSourceCount = ItemsSource.Count(); //This is the count of the "Panels" or resultGrids (Without the GridSplitter)
        var neededGridSplitter = itemsSourceCount > 1 ? itemsSourceCount + 1 : 0;
        int neededRows = itemsSourceCount + neededGridSplitter;
        int currentContentItem = 0;
        var gridSplitterHeight = 5;

        for (int i = 0; i < neededRows; i++)
        {
            var rowDefinition = new RowDefinition();
            if (i % 2 == 0)
                rowDefinition.Height = new GridLength(200);
            else
                rowDefinition.Height = new GridLength(gridSplitterHeight);
            mainGrid.RowDefinitions.Add(rowDefinition);
        }


        for (int i = 0; i < neededRows; i++)
        {
            //We start from 0 
            //Panel, GridSplitter, Panel, GridSplitter and so on
            if (i % 2 == 0)
            {
                if (ItemsSource.Count() >= currentContentItem + 1)
                {
                    var itemsSourceItem = ItemsSource.ElementAt(currentContentItem);
                    currentContentItem++;
                    if (itemsSourceItem != null && itemsSourceItem is FrameworkElement elem)
                    {
                        try
                        {
                            mainGrid.Children.Add(elem);
                            Grid.SetRow(elem, i);
                        }
                        catch (Exception ex)
                        {
                            //TODO Add new Tab, execute query, open settings, close settings tab, we reach this catch block, whyyy??????
                        }

                    }
                }
            }
            else
            {
                var gridSplitter = new GridSplitter()
                {
                    Height = gridSplitterHeight,
                    ResizeDirection = GridResizeDirection.Rows,
                };
                try
                {
                    mainGrid.Children.Add(gridSplitter);
                    Grid.SetRow(gridSplitter, i);
                }
                catch (Exception ex)
                {
                    //TODO Add new Tab, execute query, open settings, close settings tab, we reach this catch block, whyyy??????
                }
            }
        }
    }

}