using DBison.Core.Attributes;
using DBison.WPF.ClientBaseClasses;
using System.ComponentModel.DataAnnotations;

namespace DBison.WPF.ViewModels;
public class SettingItemViewModel : ClientViewModelBase
{
    public SettingItemViewModel(SettingAttribute settingAttribute, RangeAttribute rangeAttribute, object value)
    {
        SettingAttribute = settingAttribute;
        RangeAttribute = rangeAttribute;
        SettingType = SettingAttribute.Type;
        Header = SettingAttribute.Header;
        if (SettingType == typeof(uint) || SettingType == typeof(int))
            Value = Convert.ToDouble(value);
        else
            Value = value;

        if (rangeAttribute != null)
        {
            Minimum = Convert.ToDouble(rangeAttribute.Minimum);
            Maximum = Convert.ToDouble(rangeAttribute.Maximum);
        }
        else
        {
            Minimum = 0;
            Maximum = double.MaxValue;
        }
    }

    public SettingAttribute SettingAttribute
    {
        get => Get<SettingAttribute>();
        set => Set(value);
    }

    public RangeAttribute RangeAttribute
    {
        get => Get<RangeAttribute>();
        set => Set(value);
    }

    public Type SettingType
    {
        get => Get<Type>();
        set => Set(value);
    }

    public string Header
    {
        get => Get<string>();
        set => Set(value);
    }

    public object Value
    {
        get => Get<object>();
        set => Set(value);
    }

    public double Minimum
    {
        get => Get<double>();
        set => Set(value);
    }

    public double Maximum
    {
        get => Get<double>();
        set => Set(value);
    }

}
