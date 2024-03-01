namespace DBison.WPF.ClientBaseClasses
{
    public abstract class TabItemViewModelBase : ClientViewModelBase
    {
        protected TabItemViewModelBase(bool setting)
        {
            Setting = setting;
        }

        public bool Setting
        {
            get => Get<bool>();
            private set => Set(value);
        }

        #region [Header]
        public string Header
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

    }
}
