using System.Windows.Input;

namespace DBison.Core.Utils.Commands;
public class RelayCommand : ICommand
{
    private readonly Action<object> m_Execute;
    private readonly Predicate<object> m_CanExecute;

    public RelayCommand(Action<object> execute)
      : this(execute, null)
    {

    }
    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        m_Execute = execute;
        m_CanExecute = canExecute;
    }

    public RelayCommand()
    {
    }

    public bool CanExecute(object parameter)
    {
        return m_CanExecute == null ? true : m_CanExecute(parameter);
    }


    public void Execute(object parameter)
    {
        if (CanExecute(parameter))
            m_Execute(parameter);
    }

    public void OnCanExecuteChanged()
    {
        if (CanExecuteChanged != null)
            CanExecuteChanged(this, EventArgs.Empty);
    }

}