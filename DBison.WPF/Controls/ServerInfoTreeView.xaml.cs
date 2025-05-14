using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ServerInfoTreeView.xaml
    /// </summary>
    public partial class ServerInfoTreeView : UserControl
    {
        #region [ServerInfoTreeView]
        public ServerInfoTreeView()
        {
            InitializeComponent();
        }
        #endregion

        #region [TreeView]
        public TreeView TreeView => ThisTreeView;
        #endregion

        #region [__PreviewMouseButtonDown]
        private void __PreviewMouseButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //PrevewMouseButton Down => Handles the selection of the node before contextmenu open. 
            //Otherwise its confusing
            if (e.OriginalSource is DependencyObject dependency)
            {
                TreeViewItem treeViewItem = __GetTreeViewItem(dependency);
                if (treeViewItem != null)
                {
                    treeViewItem.IsSelected = true;
                }
            }
        }
        #endregion

        #region [__GetTreeViewItem]
        private TreeViewItem __GetTreeViewItem(DependencyObject source)
        {
            while (source is not null and not TreeViewItem)
                source = System.Windows.Media.VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
        #endregion

        #region [__Loaded]
        private void __Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.ServerItemsTreeView = ThisTreeView;
            }
            else
            {
                throw new Exception($"Datacontext is not matching {nameof(MainWindowViewModel)}");
            }
        }
        #endregion
    }
}
