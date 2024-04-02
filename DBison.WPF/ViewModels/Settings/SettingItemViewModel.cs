using DBison.Core.Attributes;
using DBison.Core.Entities.Enums;
using DBison.Core.Extender;
using DBison.Core.Utils.Commands;
using DBison.WPF.ClientBaseClasses;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace DBison.WPF.ViewModels;
public class SettingItemViewModel : ClientViewModelBase
{
    #region - needs -
    private bool m_Loaded = false;

    private RelayCommand m_OpenFolderDialog;
    #endregion

    #region - ctor -
    public SettingItemViewModel(SettingAttribute settingAttribute, RangeAttribute rangeAttribute, PropertyInfo propertyInfo)
    {
        __Init(settingAttribute, rangeAttribute, propertyInfo);
    }
    #endregion

    #region [SettingVisibility]
    public Visibility SettingVisibility
    {
        get => Get<Visibility>();
        set => Set(value);
    }
    #endregion

    #region [SettingsPropertyInfo]
    public PropertyInfo SettingsPropertyInfo
    {
        get => Get<PropertyInfo>();
        set => Set(value);
    }
    #endregion

    #region [SettingAttribute]
    public SettingAttribute SettingAttribute
    {
        get => Get<SettingAttribute>();
        set => Set(value);
    }
    #endregion

    #region [RangeAttribute]
    public RangeAttribute RangeAttribute
    {
        get => Get<RangeAttribute>();
        set => Set(value);
    }
    #endregion

    #region [SettingType]
    public Type SettingType
    {
        get => Get<Type>();
        set => Set(value);
    }
    #endregion

    #region [Name]
    public string Name
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [Tooltip]
    public string Tooltip
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [Value]
    public object Value
    {
        get => Get<object>();
        set
        {
            Set(value);
            if (SettingsPropertyInfo != null && m_Loaded)
            {
                if (value != null && value.GetType() != SettingType)
                    SettingsPropertyInfo.SetValue(SettingsPropertyInfo, Convert.ChangeType(value, SettingType));
                else
                    SettingsPropertyInfo.SetValue(SettingsPropertyInfo, value);
            }
        }
    }
    #endregion

    #region [Minimum]
    public double Minimum
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region [Maximum]
    public double Maximum
    {
        get => Get<double>();
        set => Set(value);
    }
    #endregion

    #region [StringStyleVariation]
    public eStringStyleVariation StringStyleVariation
    {
        get => Get<eStringStyleVariation>();
        set => Set(value);
    }
    #endregion

    #region [ListValues]
    public List<object> ListValues
    {
        get => Get<List<object>>();
        set => Set(value);
    }
    #endregion

    #region [Execute_OpenFolderDialog]
    public void Execute_OpenFolderDialog()
    {
        __OpenFolderDialog();
    }
    #endregion

    #region [__Init]
    private void __Init(SettingAttribute settingAttribute, RangeAttribute rangeAttribute, PropertyInfo propertyInfo)
    {
        SettingsPropertyInfo = propertyInfo;
        var value = propertyInfo.GetValue(settingAttribute);
        SettingAttribute = settingAttribute;
        RangeAttribute = rangeAttribute;
        SettingType = SettingAttribute.Type;
        Name = SettingAttribute.Header;
        Tooltip = SettingAttribute.ToolTip;
        StringStyleVariation = SettingAttribute.StringStyleVariation;
        if (SettingType == typeof(uint) || SettingType == typeof(int))
        {
            Value = Convert.ToDouble(value);
        }
        else if (SettingType == typeof(Enum))
        {
            var values = Enum.GetValues(propertyInfo.PropertyType);
            ListValues = new List<object>();
            if (values != null)
            {
                foreach (var enumvalue in values)
                {
                    ListValues.Add(enumvalue);
                }
            }
            Value = value;
        }
        else
        {
            Value = value;
        }

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
    #endregion

    #region [__OpenFolderDialog]
    private void __OpenFolderDialog()
    {
        using FolderBrowserDialog dialog = new();
        dialog.AddToRecent = false;
        dialog.OkRequiresInteraction = true;
        dialog.Description = "Select the path of the plugins folder";
        dialog.InitialDirectory = Environment.CurrentDirectory;
        dialog.ShowNewFolderButton = true;
        dialog.UseDescriptionForTitle = true;
        var result = dialog.ShowDialog();
        if (result == DialogResult.OK)
            Value = dialog.SelectedPath;
    }
    #endregion

    #region [EvaluateDependencies]
    internal void EvaluateDependencies(IEnumerable<SettingItemViewModel> allSettings)
    {
        var dependencies = SettingsPropertyInfo.GetCustomAttributes(typeof(DependsUponSettingAttribute), false);

        var myName = SettingsPropertyInfo.Name;
        bool isShown = true;

        foreach (var dependencyTempAttribute in dependencies)
        {
            if (!isShown)
            {
                break;
            }
            if (dependencyTempAttribute is DependsUponSettingAttribute settingsAttribute)
            {
                var otherSetting = allSettings.FirstOrDefault(s => s.SettingsPropertyInfo.Name.IsEquals(settingsAttribute.DependingSetting));
                if (otherSetting != null)
                {
                    var xBool = Convert.ToBoolean(otherSetting.Value);
                    if (!settingsAttribute.InverseBoolValue && !xBool)
                        isShown = false;
                    else if (settingsAttribute.InverseBoolValue && xBool)
                        isShown = false;
                }
            }
        }
        SettingVisibility = isShown && SettingAttribute.IsVisible ? Visibility.Visible : Visibility.Collapsed;
    }
    #endregion
}
