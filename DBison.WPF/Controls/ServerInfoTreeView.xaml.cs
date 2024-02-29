using System.Windows.Controls;

namespace DBison.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ServerInfoTreeView.xaml
    /// </summary>
    public partial class ServerInfoTreeView : UserControlBase
    {
        public ServerInfoTreeView()
        {
            InitializeComponent();
        }

        public TreeView TreeView => ThisTreeView;
    }
}
