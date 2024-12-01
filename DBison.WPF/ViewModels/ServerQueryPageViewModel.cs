using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using DBison.Core.Utils.SettingsSystem;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Converter;
using DBison.WPF.HelperObjects;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace DBison.WPF.ViewModels;
public class ServerQueryPageViewModel : TabItemViewModelBase
{
    #region - needs -
    ServerViewModel m_ServerViewModel;
    ServerQueryHelper m_ServerQueryHelper;
    DispatcherTimer m_ExecutionTimer;
    Stopwatch m_Stopwatch = new Stopwatch();

    private int m_ExecutingSQLCount;
    private int m_ExecutingResultCount;
    private List<TimeSpan> m_ExecutingResultTimesSpans = new();
    private static object m_Locker = new object();

    #endregion

    #region Ctor
    public ServerQueryPageViewModel(QueryPageCreationReq req)
        : base(false)
    {
        DatabaseObject = req.DataBaseObject;
        m_ServerViewModel = req.ServerViewModel;
        Header = req.Name;
        ResultSets = new ObservableCollection<DataGrid>();
        m_ServerQueryHelper = req.ServerQueryHelper;
        if (req.QueryText.IsNotNullOrEmpty())
        {
            QueryText = req.QueryText;
            FillDataTable(QueryText, DatabaseObject.DataBase);
        }
    }
    #endregion

    #region - properties -
    #region - public properties -

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
    public ObservableCollection<DataGrid> ResultSets
    {
        get => Get<ObservableCollection<DataGrid>>();
        set => Set(value);
    }
    #endregion

    #region [QueryStatisticText]
    public string QueryStatisticText
    {
        get => Get<string>();
        set => Set(value);
    }
    #endregion

    #region [MaxHeight]
    [DependsUpon(nameof(ResultSets))]
    public double MaxHeight => ResultSets.Count > 1 ? 100 : double.MaxValue;
    #endregion

    #endregion
    #endregion

    #region - commands -

    #region [Execute_Close]
    public override void Execute_Close()
    {
        m_ServerViewModel.RemoveQuery(this);
    }
    #endregion

    #region [CanExecute_ExecuteSQL]
    [DependsUpon(nameof(QueryText))]
    [DependsUpon(nameof(SelectedQueryText))]
    [DependsUpon(nameof(IsLoading))]
    public bool CanExecute_ExecuteSQL()
    {
        if (IsLoading || DatabaseObject.DataBase.DataBaseState != eDataBaseState.ONLINE)
            return false;
        return (SelectedQueryText != null && SelectedQueryText.Trim().IsNotNullOrEmpty()) || (QueryText != null && QueryText.Trim().IsNotNullOrEmpty());
    }
    #endregion

    #region [Execute_ExecuteSQL]
    public void Execute_ExecuteSQL()
    {
        if (DatabaseObject is DatabaseInfo dbInfo)
            FillDataTable(SelectedQueryText.IsNotNullOrEmpty() ? SelectedQueryText : QueryText, dbInfo);
    }
    #endregion

    #region [CanExecute_ClearResult]
    [DependsUpon(nameof(IsLoading))]
    [DependsUpon(nameof(ResultSets))]
    public bool CanExecute_ClearResult()
    {
        return !IsLoading && ResultSets.IsNotEmpty();
    }
    #endregion

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
        if (!IsLoading)
            return;
        m_ServerQueryHelper.Cancel();
        IsLoading = false;
        QueryStatisticText = string.Empty;
    }
    #endregion

    #region [Execute_SaveQueryAs]
    public void Execute_SaveQueryAs()
    {
        if (QueryText == null || QueryText.Trim().IsNullOrEmpty())
            return;

        var textToExport = QueryText;

        var dlg = new SaveFileDialog()
        {
            AddExtension = true,
            AddToRecent = true,
            DefaultExt = "sql",
            FileName = "SQLQuery",
            Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*",
        };
        dlg.ShowDialog();
        if (dlg.FileName.IsNotNullOrEmpty())
            File.WriteAllText(dlg.FileName, textToExport);
    }
    #endregion

    #endregion

    #region - public methods -
    #region [FillDataTable]
    public void FillDataTable(string sql, DatabaseInfo databaseInfo)
    {
        if (sql.IsNullOrEmpty())
            return;
        bool clearResultBeforeExecuteNewQuery = true;

        var sqls = sql.ExtractStatements().Where(s => s.IsNotNullOrEmpty());

        if (sqls.IsEmpty())
        {
            QueryStatisticText = "Ready";
            return;
        }

        if (clearResultBeforeExecuteNewQuery)
        {
            ResultSets.Clear();
            QueryStatisticText = "Executing...";
        }

        IsLoading = true;

        m_ExecutingSQLCount = sqls.Count();
        m_ExecutingResultCount = 0;
        m_ExecutingResultTimesSpans.Clear();

        foreach (var singleSql in sqls)
        {
            new Task(() =>
            {
                lock (m_Locker)
                {
                    __ExecuteQuery(singleSql, databaseInfo);
                }
            }).Start();
        }
    }
    #endregion
    #endregion

    #region - private methods -

    #region [__ExecuteQuery]
    private void __ExecuteQuery(string singleSql, DatabaseInfo databaseInfo)
    {
        __PrepareTimer(() =>
        {
            var dataTable = m_ServerQueryHelper.FillDataTable(databaseInfo, singleSql.ToStringValue(), __Error);
            if (dataTable == null)
            {
                __CleanTimer();
                __HandleSQLExecutionDone();
                return;
            }
            ExecuteOnDispatcher(() =>
            {
                var dataGrid = new DataGrid
                {
                    GridLinesVisibility = (DataGridGridLinesVisibility)Settings.DataGridGridLinesVisibility, //Todo as setting CBO
                    IsReadOnly = true,
                    Margin = new System.Windows.Thickness(0, 10, 0, 10),
                    ItemsSource = dataTable.DefaultView
                };
                dataGrid.AutoGeneratingColumn += __AutoGeneratingColumn;
                OnPropertyChanged(nameof(MaxHeight));
                ResultSets.Add(dataGrid);
                __HandleSQLExecutionDone();
                OnPropertyChanged(nameof(ResultSets));
                __CleanTimer();
            });
        });
    }

    private void __HandleSQLExecutionDone()
    {
        ExecuteOnDispatcher(() =>
        {
            m_ExecutingResultCount++;
            if (m_ExecutingResultCount >= m_ExecutingSQLCount)
            {
                IsLoading = false;
            }
            QueryStatisticText = $"Query Exection {m_ExecutingResultCount}/{m_ExecutingSQLCount} done. Elapsed time {m_ExecutingResultTimesSpans.Sum():m\\:ss\\.ffff} minutes";
        });
    }

    #endregion

    #region [__PrepareTimer]
    private void __PrepareTimer(Action onTimerPrepared)
    {
        ExecuteOnDispatcher(() =>
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

    #region [__CleanTimer]
    private void __CleanTimer()
    {
        ExecuteOnDispatcher(() =>
        {
            m_ExecutionTimer?.Stop();
            m_Stopwatch?.Stop();
            if (m_Stopwatch != null)
                m_ExecutingResultTimesSpans.Add(m_Stopwatch.Elapsed);
        });
    }
    #endregion

    #region [__ExecutionTimer_Tick]
    private void __ExecutionTimer_Tick(object? sender, EventArgs e)
    {
        ExecuteOnDispatcher(() =>
        {
            QueryStatisticText = $"Executing - " + m_Stopwatch.Elapsed.ToString(@"mm\:ss\.ffff");
        });
    }
    #endregion

    #region [__Error]
    private void __Error(Exception ex)
    {
        QueryStatisticText = $"ERROR occured";
        m_ServerViewModel.ExecuteError(ex);
    }
    #endregion

    #region [__AutoGeneratingColumn]
    private void __AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var column = (e.Column as DataGridBoundColumn);

        var binding = new Binding(e.PropertyName);
        binding.Converter = new EmptyValueConverter();
        column.Binding = binding;
    }
    #endregion

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (!disposing || IsDisposed)
            return;

        Execute_CancelQuery();

        if (m_ExecutionTimer != null)
        {
            m_ExecutionTimer.Tick -= __ExecutionTimer_Tick;
            m_ExecutionTimer = null;
        }

        base.Dispose(disposing);
    }
}
