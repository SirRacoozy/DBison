using System.Windows.Input;

namespace DBison.Core.Utils.Commands;
public class RelayCommand : ICommand
{
    private readonly Action<object> m_Execute;

    private readonly Predicate<object> m_CanExecute;

    #region [RelayCommand]
    public RelayCommand(Action<object> execute)
      : this(execute, null)
    {

    }
    #endregion

    #region [RelayCommand]
    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        m_Execute = execute;
        m_CanExecute = canExecute;
    }
    #endregion

    #region [RelayCommand]
    public RelayCommand()
    {
    }
    #endregion

    #region [CanExecute]
    public bool CanExecute(object parameter)
    {
        return m_CanExecute == null ? true : m_CanExecute(parameter);
    }
    #endregion

    #region [Execute]
    public void Execute(object parameter)
    {
        if (CanExecute(parameter))
            m_Execute(parameter);
    }
    #endregion

    #region [OnCanExecuteChanged]
    public virtual void OnCanExecuteChanged()
    {
        if (CanExecuteChanged != null)
            CanExecuteChanged(this, EventArgs.Empty);
    }
    #endregion

    public event EventHandler CanExecuteChanged;
}