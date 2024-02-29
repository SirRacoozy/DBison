using DBison.Core.Attributes;
using DBison.WPF.ClientBaseClasses;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DBison.WPF.ViewModels;
public class SettingItemViewModel : ClientViewModelBase
{
    private bool m_Loaded = false;
    public SettingItemViewModel(SettingAttribute settingAttribute, RangeAttribute rangeAttribute, PropertyInfo propertyInfo/* object value*/)
    {
        __Init(settingAttribute, rangeAttribute, propertyInfo);
    }

    public PropertyInfo SettingsProperty
    {
        get => Get<PropertyInfo>();
        set => Set(value);
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

    public string Tooltip
    {
        get => Get<string>();
        set => Set(value);
    }

    public object Value
    {
        get => Get<object>();
        set
        {
            Set(value);
            if (SettingsProperty != null && m_Loaded)
            {
                if (value.GetType() != SettingType)
                    SettingsProperty.SetValue(SettingsProperty, Convert.ChangeType(value, SettingType));
                else
                    SettingsProperty.SetValue(SettingsProperty, value);
            }
        }
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

    private void __Init(SettingAttribute settingAttribute, RangeAttribute rangeAttribute, PropertyInfo propertyInfo)
    {
        SettingsProperty = propertyInfo;
        var value = propertyInfo.GetValue(settingAttribute);
        SettingAttribute = settingAttribute;
        RangeAttribute = rangeAttribute;
        SettingType = SettingAttribute.Type;
        Header = SettingAttribute.Header;
        Tooltip = SettingAttribute.ToolTip;
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
        m_Loaded = true;
    }

}
