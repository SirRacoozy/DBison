namespace DBison.WPF.ClientBaseClasses
{
    public abstract class TabItemViewModelBase : ClientViewModelBase
    {
        #region [TabItemViewModelBase]
        protected TabItemViewModelBase(bool setting)
        {
            Setting = setting;
        }
        #endregion

        #region [Setting]
        public bool Setting
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [Header]
        public string Header
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [Execute_Close]
        public abstract void Execute_Close();
        #endregion
    }
}
