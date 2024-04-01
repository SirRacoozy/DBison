using System.Windows.Controls;

namespace DBison.WPF.Controls;
public class TabControlBase : TabControl
{
    #region [TabControlBase]
    public TabControlBase()
    {
        __OverrideStyle();
    }
    #endregion

    #region [OnApplyTemplate]
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }
    #endregion

    #region [__OverrideStyle]
    private void __OverrideStyle()
    {

    }
    #endregion
}
