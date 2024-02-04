using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace DBison.WPF.Controls;
public class TabControlBase : TabControl
{
    public TabControlBase()
    {
        __OverrideStyle();
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    private void __OverrideStyle()
    {

    }
}
