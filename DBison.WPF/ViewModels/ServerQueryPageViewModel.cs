using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using DBison.WPF.ClientBaseClasses;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace DBison.WPF.ViewModels;
public class ServerQueryPageViewModel : ClientViewModelBase
{
    ServerViewModel m_ServerViewModel;
    ServerQueryHelper m_ServerQueryHelper;
    DispatcherTimer m_ExecutionTimer;
    Stopwatch m_Stopwatch = new Stopwatch();

    #region Ctor
    public ServerQueryPageViewModel(string name, ServerViewModel serverViewModel, DatabaseObjectBase databaseObject, ServerQueryHelper serverQueryHelper)
    {
        DatabaseObject = databaseObject;
        m_ServerViewModel = serverViewModel;
        Header = name;
        ResultSets = new ObservableCollection<ResultSetViewModel>();
        m_ServerQueryHelper = serverQueryHelper;
    }
    #endregion

    #region - properties -
    #region - public properties -
    #region [Header]
    public string Header
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [SelectedQueryText]
    public string SelectedQueryText
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [QueryText]
    public string QueryText
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [ResultOnly]
    public bool ResultOnly
    {
        get => Get<bool>();
        set => Set(value);
    }
    #endregion

    #region [IsLoading]
    public bool IsLoading
    {
        get => Get<bool>();
        set => Set(value);
    }
    #endregion

    #region [DatabaseObject]
    public DatabaseObjectBase DatabaseObject
    {
        get => Get<DatabaseObjectBase>();
        set => Set(value);
    }
    #endregion

    #region [ResultSets]
    public ObservableCollection<ResultSetViewModel> ResultSets
    {
        get => Get<ObservableCollection<ResultSetViewModel>>();
        set => Set(value);
    }
    #endregion

    #region [QueryVisibility]
    [DependsUpon(nameof(ResultOnly))]
    public Visibility QueryVisibility => ResultOnly ? Visibility.Collapsed : Visibility.Visible;
    #endregion

    #region [ResultGridsRow]
    [DependsUpon(nameof(ResultOnly))]
    public int ResultGridsRow => ResultOnly ? 0 : 2;
    #endregion

    #region [ResultGridsRowSpan]
    [DependsUpon(nameof(ResultOnly))]
    public int ResultGridsRowSpan => ResultOnly ? 3 : 1;
    #endregion

    #region [ResultGridsMaxHeight]
    [DependsUpon(nameof(ResultOnly))]
    public double ResultGridsMaxHeight => ResultOnly ? 300 : double.NaN;
    #endregion

    #region [QueryStatisticText]
    public string QueryStatisticText
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #endregion
    #endregion

    #region - commands -
    #region [Execute_Close]
    public void Execute_Close()
    {
        m_ServerViewModel.RemoveQuery(this);
    }
    #endregion

    [DependsUpon(nameof(QueryText))]
    [DependsUpon(nameof(SelectedQueryText))]
    [DependsUpon(nameof(IsLoading))]
    public bool CanExecute_ExecuteSQL()
    {
        if (IsLoading)
            return false;
        return (SelectedQueryText != null && SelectedQueryText.Trim().IsNotNullOrEmpty()) || (QueryText != null && QueryText.Trim().IsNotNullOrEmpty());
    }

    #region [Execute_ExecuteSQL]
    public void Execute_ExecuteSQL()
    {
        if (DatabaseObject is DatabaseInfo dbInfo)
            FillDataTable(SelectedQueryText.IsNotNullOrEmpty() ? SelectedQueryText : QueryText, dbInfo);
    }
    #endregion

    [DependsUpon(nameof(IsLoading))]
    [DependsUpon(nameof(ResultSets))]
    public bool CanExecute_ClearResult()
    {
        return !IsLoading && ResultSets.Any(r => r.ResultLines.Count != 0);
    }

    #region [Execute_ClearResult]
    public void Execute_ClearResult()
    {
        ResultSets.Clear();
        OnPropertyChanged(nameof(IsLoading));
        QueryStatisticText = string.Empty;
    }
    #endregion

    #region [CanExecute_CancelQuery]
    [DependsUpon(nameof(IsLoading))]
    public bool CanExecute_CancelQuery()
    {
        return IsLoading;
    }
    #endregion

    #region [Execute_CancelQuery]
    public void Execute_CancelQuery()
    {
        m_ServerQueryHelper.Cancel();
        IsLoading = false;
    }
    #endregion

    #endregion

    #region - public methods -
    #region [FillDataTable]
    public void FillDataTable(string sql, DatabaseInfo databaseInfo)
    {
        if (sql.IsNullOrEmpty())
            return;
        bool clearResultBeforeExecuteNewQuery = true; //Display only one, because we have virtualisation problems

        if (clearResultBeforeExecuteNewQuery) //TODO: Setting
        {
            ResultSets.Clear();
            QueryStatisticText = "Executing...";
        }
        var sqls = sql.Split(";").Where(s => s.IsNotNullOrEmpty()).Take(1); //First simple way to separate sqls. search another way with regex

        if (sqls.Any())
            IsLoading = true;

        foreach (var singleSql in sqls)
        {
            new Task(() =>
            {
                __ExecuteQuery(databaseInfo, singleSql);
            }).Start();
        }
    }
    #endregion
    #endregion

    #region - private methods -

    #region [__ExecuteOnDispatcher]
    private void __ExecuteOnDispatcher(Action actionToExecute)
    {
        _ = System.Windows.Application.Current.Dispatcher.BeginInvoke(actionToExecute);
    }
    #endregion

    #region [__ExecuteQuery]
    private void __ExecuteQuery(DatabaseInfo databaseInfo, string singleSql)
    {
        __PrepareTimer(() =>
        {
            var dataTable = m_ServerQueryHelper.FillDataTable(databaseInfo, singleSql.ToStringValue(), __Error);

            __ExecuteOnDispatcher(() => IsLoading = false);

            if (dataTable == null)
            {
                __CleanTimer();
                return;
            }
            __ExecuteOnDispatcher(() =>
             ResultSets.Add(new ResultSetViewModel()
             {
                 ResultLines = dataTable.DefaultView
             }));
            __CleanTimer();
            QueryStatisticText = $"Query executed in {m_Stopwatch.Elapsed.ToString(@"m\:ss\.ffff")} minutes - {dataTable.Rows.Count.ToString("N0")} Rows";
        });
    }

    #endregion

    #region [__PrepareTimer]
    private void __PrepareTimer(Action onTimerPrepared)
    {
        __ExecuteOnDispatcher(() =>
        {
            if (m_ExecutionTimer == null)
            {
                m_ExecutionTimer = new DispatcherTimer();
                m_ExecutionTimer.Interval = TimeSpan.FromSeconds(1);
                m_ExecutionTimer.Tick += __ExecutionTimer_Tick;
            }
            m_Stopwatch = new Stopwatch();
            m_ExecutionTimer?.Stop();
            m_ExecutionTimer?.Start();
            m_Stopwatch.Start();
        });
        onTimerPrepared?.Invoke();
    }
    #endregion

    private void __CleanTimer()
    {
        __ExecuteOnDispatcher(() =>
        {
            m_ExecutionTimer?.Stop();
            m_Stopwatch?.Stop();
        });
    }

    private void __ExecutionTimer_Tick(object? sender, EventArgs e)
    {
        __ExecuteOnDispatcher(() =>
        {
            QueryStatisticText = $"Executing - " + m_Stopwatch.Elapsed.ToString(@"mm\:ss\.ffff");
        });
    }


    private void __Error(Exception ex)
    {
        QueryStatisticText = $"ERROR occured";
        m_ServerViewModel.ExecuteError(ex);
    }

    #endregion
}
