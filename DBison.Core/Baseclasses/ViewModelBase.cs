using DBison.Core.Attributes;
using DBison.Core.Extender;
using DBison.Core.Utils.Commands;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DBison.Core.Baseclasses;
public class ViewModelBase : INotifyPropertyChanged, IDisposable
{
    private const string m_EXECUTE_PREFIX = "Execute_";
    private const string m_CANEXECUTE_PREFIX = "CanExecute_";
    private bool m_SkipCheckingDependendMembers;

    public event PropertyChangedEventHandler PropertyChanged;
    private readonly ConcurrentDictionary<string, object> m_Properties;
    private readonly ConcurrentDictionary<string, object> m_Values;

    private readonly List<string> m_CommandNames;
    private IDictionary<string, MethodInfo> m_Methods;
    private IDictionary<string, DependsUponObject> m_DependsUponDict;
    public bool IsDisposed;

    public ViewModelBase()
    {
        m_Properties = new ConcurrentDictionary<string, object>();
        m_DependsUponDict = new ConcurrentDictionary<string, DependsUponObject>();
        m_CommandNames = new List<string>();

        Type MyType = GetType();
        m_Values = new ConcurrentDictionary<string, object>();
        __GetMembersAndGenerateCommands(MyType);
    }

    public object this[string key]
    {
        get
        {
            if (m_Values.ContainsKey(key))
                return m_Values[key];
            return null;
        }
        set
        {
            m_Values[key] = value;
            OnPropertyChanged(key);
        }
    }

    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool HasChanges
    {
        get => Get<bool>();
        set => Set(value);
    }

    #region Get
    protected T Get<T>(Expression<Func<T>> expression)
    {
        return Get<T>(__GetPropertyName(expression));
    }
    protected T Get<T>(Expression<Func<T>> expression, T defaultValue)
    {
        return Get(__GetPropertyName(expression), defaultValue);
    }

    protected T Get<T>(T defaultValue, [CallerMemberName] string propertyName = null)
    {
        return Get(propertyName, defaultValue);
    }
    protected T Get<T>([CallerMemberName] string name = null)
    {
        return Get(name, default(T));
    }

    protected T Get<T>(string name, T defaultValue)
    {
        return GetValueByName<T>(name, defaultValue);
    }

    protected T GetValueByName<T>(String name, T defaultValue)
    {

        if (m_Properties.TryGetValue(name, out var val))
            return (T)val;

        return defaultValue;
    }
    #endregion

    #region [Set]

    protected void Set<T>(Expression<Func<T>> expression, T value)
    {
        Set(__GetPropertyName(expression), value);
    }

    protected void Set<T>(T value, [CallerMemberName] string propertyName = "")
    {
        Set(propertyName, value);
    }

    public void Set<T>(string name, T value)
    {
        if (m_Properties.TryGetValue(name, out var val))
        {
            if (val == null && value == null)
                return;

            if (val != null && val.Equals(value))
                return;
        }
        m_Properties[name] = value;
        this[name] = value;
        OnPropertyChanged(name);
        if (!m_SkipCheckingDependendMembers)
            __RefreshDependendObjects(name);
        m_CommandNames.ForEach(name => OnCanExecuteChanged(name));
        if (name != nameof(HasChanges))
            HasChanges = true;
    }
    #endregion

    public void ExecuteWithoutDependendObjects(Action action)
    {
        try
        {
            m_SkipCheckingDependendMembers = true;
            action?.Invoke();
            m_SkipCheckingDependendMembers = false;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            m_SkipCheckingDependendMembers = false;
        }
    }

    public List<string> GetCommandNames()
    {
        return m_CommandNames;
    }

    public virtual void OnCanExecuteChanged(string commandName)
    {
        try
        {
            var command = Get<RelayCommand>(commandName);
            if (command != null)
                command.OnCanExecuteChanged();
        }
        catch (Exception)
        {
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private string __GetPropertyName<T>(Expression<Func<T>> expression)
    {
        if (expression.Body is MemberExpression memberExpr)
            return memberExpr.Member.Name;
        return string.Empty;
    }

    private void __GetMembersAndGenerateCommands(Type myType)
    {
        var MethodInfos = new Dictionary<String, MethodInfo>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var method in myType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (method.Name.StartsWith(m_EXECUTE_PREFIX))
                m_CommandNames.Add(method.Name.Substring(m_EXECUTE_PREFIX.Length));
            __ProcessMethodAttributes(method);
            MethodInfos[method.Name] = method;
        }
        foreach (var property in myType.GetProperties())
        {
            this[property.Name] = property;
            __ProcessPropertyAttributes(property);
        }
        m_CommandNames.ForEach(n => Set(n, new RelayCommand(p => __ExecuteCommand(n, p), p => __CanExecuteCommand(n, p))));
        m_Methods = MethodInfos;
    }

    private void __ProcessPropertyAttributes(PropertyInfo property)
    {
        var attributes = property.GetCustomAttributes<DependsUponAttribute>();
        if (attributes.Any())
            m_DependsUponDict[property.Name] = new DependsUponObject { DependendObjects = attributes.Where(a => a.MemberName.IsNotNullOrEmpty()).Select(m => m.MemberName).ToList() };
    }

    private void __ProcessMethodAttributes(MethodInfo method)
    {
        var attributes = method.GetCustomAttributes<DependsUponAttribute>();
        if (attributes.Any())
            m_DependsUponDict[method.Name] = new DependsUponObject { DependendObjects = attributes.Where(a => a.MemberName.IsNotNullOrEmpty()).Select(m => m.MemberName).ToList() };
    }

    private void __ExecuteCommand(string name, object parameter)
    {
        _ = m_Methods.TryGetValue(m_EXECUTE_PREFIX + name, out MethodInfo methodInfo);
        if (methodInfo == null)
            return;
        _ = methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] { parameter } : null);
    }

    private bool __CanExecuteCommand(string name, object parameter)
    {
        _ = m_Methods.TryGetValue(m_CANEXECUTE_PREFIX + name, out MethodInfo methodInfo);
        if (methodInfo == null)
            return true;

        return (bool)methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] { parameter } : null);
    }

    private void __RefreshDependendObjects(string memberName)
    {
        if (m_DependsUponDict != null)
        {
            var dependendObjects = m_DependsUponDict.Where(d => d.Value != null && d.Value.DependendObjects != null && d.Value.DependendObjects.Contains(memberName));
            if (dependendObjects != null)
            {
                foreach (var dependsUponObj in dependendObjects)
                {
                    if (dependsUponObj.Value.DependendObjects != null && dependsUponObj.Value.DependendObjects.Any())
                    {
                        if (m_Properties.ContainsKey(dependsUponObj.Key))
                        {
                            OnPropertyChanged(dependsUponObj.Key);
                        }
                        else if (m_Methods.ContainsKey(dependsUponObj.Key))
                        {
                            MethodInfo methodInfo;
                            m_Methods.TryGetValue(dependsUponObj.Key, out methodInfo);
                            if (methodInfo == null) return;
                            if (methodInfo.GetParameters().Length == 0)
                            {
                                try
                                {
                                    methodInfo.Invoke(this, null);
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }
                        else
                        {
                            OnPropertyChanged(dependsUponObj.Key);
                        }
                    }
                }
            }
        }
    }
}