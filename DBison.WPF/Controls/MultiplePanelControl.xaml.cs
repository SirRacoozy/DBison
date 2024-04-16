﻿using DBison.Core.Extender;
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
        __UpdateItemsSource();
    }

    private void __UpdateItemsSource()
    {
        Content = new TextBlock { Text = "EMPTY", FontSize = 40, Foreground = Brushes.Red, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
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
        var itemsSourceCount = ItemsSource.Count(); //This is the count of the "Panels" or resultGrids (Without the GridSplitter)
        int neededRows = itemsSourceCount + itemsSourceCount - 1;
        int currentContentItem = 0;

        for (int i = 0; i < neededRows; i++)
        {
            var rowDefinition = new RowDefinition();
            if (i % 2 == 0)
            {
                rowDefinition.Height = GridLength.Auto;
            }
            else
            {
                rowDefinition.Height = new GridLength(5);
            }
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
                        if (itemsSourceCount != 1)
                            elem.MaxHeight = 200;
                        mainGrid.Children.Add(elem);
                        Grid.SetRow(elem, i);
                    }
                }
            }
            else
            {
                var gridSplitter = new GridSplitter()
                {
                    Height = 5,
                    ResizeDirection = GridResizeDirection.Rows,
                };
                mainGrid.Children.Add(gridSplitter);
                Grid.SetRow(gridSplitter, i);
            }
        }
        Content = mainGrid;
    }
    #endregion

}