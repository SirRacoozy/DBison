using DBison.WPF.ClientBaseClasses;
using DBison.WPF.ViewModels;
using DBison.WPF.Views;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls
{
    public class ServerQueryPageDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement elem && item is TabItemViewModelBase vieModel)
            {
                var template = new DataTemplate();
                FrameworkElementFactory factory = null;
                if (vieModel.Setting)
                {
                    factory = new FrameworkElementFactory(typeof(SettingsTabView));
                }
                else
                {
                    factory = new FrameworkElementFactory(typeof(QueryPageView));
                }
                template.VisualTree = factory;
                return template;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
