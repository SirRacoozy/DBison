using DBison.Core.Attributes;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using DBison.WPF.ClientBaseClasses;
using DBison.WPF.Converter;
using DBison.WPF.HelperObjects;
using Microsoft.Win32;
using System.Collections.ObjectModel;
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

        //if (DatabaseObject.DataBase is DatabaseInfo dbInfo)
        //{
        //    var sql = SelectedQueryText.IsNotNullEmptyOrWhitespace() ? SelectedQueryText : QueryText;
        //    var result = sql.ConvertToSelectStatement();

        //    switch (result.Item2)
        //    {
        //        case eDMLOperator.Update:
        //            using (var Access = new DatabaseAccess(dbInfo.Server, dbInfo))
        //                Access.ExecuteCommand(sql);
        //            FillDataTable(result.Item1, dbInfo);
        //            break;
        //        case eDMLOperator.Delete:
        //            FillDataTable(result.Item1, dbInfo);
        //            using (var Access = new DatabaseAccess(dbInfo.Server, dbInfo))
        //                Access.ExecuteCommand(sql);
        //            break;
        //        case eDMLOperator.Insert:
        //            using (var Access = new DatabaseAccess(dbInfo.Server, dbInfo))
        //                Access.ExecuteCommand(sql);
        //            FillDataTable(result.Item1, dbInfo);
        //            break;
        //        default:
        //            FillDataTable(result.Item1, dbInfo);
        //            break;
        //    }
        //}
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
        bool clearResultBeforeExecuteNewQuery = true; //Display only one, because we have virtualisation problems

        if (clearResultBeforeExecuteNewQuery) //TODO: Setting
        {
            ResultSets.Clear();
            QueryStatisticText = "Executing...";
        }
        var sqls = sql.Split(";").Where(s => s.IsNotNullOrEmpty()).Take(1); //First simple way to separate sqls. search another way with regex

        if (sqls.IsNotEmpty())
            IsLoading = true;

        foreach (var singleSql in sqls)
        {
            new Task(() =>
            {
                __ExecuteQuery(singleSql, databaseInfo);
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
            var dataTables = m_ServerQueryHelper.FillDataTable(databaseInfo, singleSql.ToStringValue(), __Error);

            ExecuteOnDispatcher(() => IsLoading = false);

            var resultList = new ObservableCollection<DataGrid>();

            foreach (var dataTable in dataTables)
            {
                if (dataTable == null)
                {
                    __CleanTimer();
                    return;
                }
                ExecuteOnDispatcher(() =>
                {
                    var dataGrid = new DataGrid
                    {
                        IsReadOnly = true,
                        ItemsSource = dataTable.DefaultView
                    };
                    dataGrid.AutoGeneratingColumn += __AutoGeneratingColumn;
                    resultList.Add(dataGrid);
                    OnPropertyChanged(nameof(MaxHeight));
                    OnPropertyChanged(nameof(ResultSets));
                    if (resultList.Count == dataTables.Count)
                        ResultSets = resultList;
                });
            }

            OnPropertyChanged(nameof(ResultSets));
            __CleanTimer();
            QueryStatisticText = $"Query executed in {m_Stopwatch.Elapsed:m\\:ss\\.ffff} minutes - {dataTables.Sum(dt => dt.Rows.Count):N0} Rows on {dataTables.Count()} ResultSets";
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
