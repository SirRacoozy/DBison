using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DBison.WPF.Controls;

[TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
public class TabControlBase : TabControl
{
    private Panel m_ItemsHolderPanel = null;

    public TabControlBase()
        : base()
    {
        // This is necessary so that we get the initial databound selected item
        ItemContainerGenerator.StatusChanged += __ItemContainerGenerator_StatusChanged;
    }

    /// <summary>
    /// If containers are done, generate the selected item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void __ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
    {
        if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
        {
            this.ItemContainerGenerator.StatusChanged -= __ItemContainerGenerator_StatusChanged;
            __UpdateSelectedItem();
        }
    }

    /// <summary>
    /// Get the ItemsHolder and generate any children
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        m_ItemsHolderPanel = GetTemplateChild("PART_ItemsHolder") as Panel;
        __UpdateSelectedItem();
    }

    /// <summary>
    /// When the items change we remove any generated panel children and add any new ones as necessary
    /// </summary>
    /// <param name="e"></param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        if (m_ItemsHolderPanel == null)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                m_ItemsHolderPanel.Children.Clear();
                break;

            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        ContentPresenter cp = __FindChildContentPresenter(item);
                        if (cp != null)
                            m_ItemsHolderPanel.Children.Remove(cp);
                    }
                }

                // Don't do anything with new items because we don't want to
                // create visuals that aren't being shown

                __UpdateSelectedItem();
                break;

            case NotifyCollectionChangedAction.Replace:
                throw new NotImplementedException("Replace not implemented yet");
        }
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        __UpdateSelectedItem();
    }

    private void __UpdateSelectedItem()
    {
        if (m_ItemsHolderPanel == null)
            return;

        // Generate a ContentPresenter if necessary
        TabItem item = GetSelectedTabItem();
        if (item != null)
            __CreateChildContentPresenter(item);

        // show the right child
        foreach (ContentPresenter child in m_ItemsHolderPanel.Children)
            child.Visibility = (child.Tag as TabItem).IsSelected ? Visibility.Visible : Visibility.Collapsed;
    }

    private ContentPresenter __CreateChildContentPresenter(object item)
    {
        if (item == null)
            return null;

        ContentPresenter cp = __FindChildContentPresenter(item);

        if (cp != null)
            return cp;

        // the actual child to be added.  cp.Tag is a reference to the TabItem
        cp = new ContentPresenter();
        cp.Content = (item is TabItem) ? (item as TabItem).Content : item;
        cp.ContentTemplate = this.SelectedContentTemplate;
        cp.ContentTemplateSelector = this.SelectedContentTemplateSelector;
        cp.ContentStringFormat = this.SelectedContentStringFormat;
        cp.Visibility = Visibility.Collapsed;
        cp.Tag = (item is TabItem) ? item : (this.ItemContainerGenerator.ContainerFromItem(item));
        m_ItemsHolderPanel.Children.Add(cp);
        return cp;
    }

    private ContentPresenter __FindChildContentPresenter(object data)
    {
        if (data is TabItem)
            data = (data as TabItem).Content;

        if (data == null)
            return null;

        if (m_ItemsHolderPanel == null)
            return null;

        foreach (ContentPresenter cp in m_ItemsHolderPanel.Children)
        {
            if (cp.Content == data)
                return cp;
        }

        return null;
    }

    protected TabItem GetSelectedTabItem()
    {
        object selectedItem = base.SelectedItem;
        if (selectedItem == null)
            return null;

        if (selectedItem is not TabItem item)
            item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as TabItem;

        return item;
    }
}
