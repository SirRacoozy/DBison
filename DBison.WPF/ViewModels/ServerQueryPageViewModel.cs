using DBison.Core.Attributes;
using DBison.Core.Baseclasses;
using DBison.Core.Entities;
using DBison.Core.Extender;
using DBison.Core.Helper;
using System.Collections.ObjectModel;
using System.Windows;

namespace DBison.WPF.ViewModels;
public class ServerQueryPageViewModel : ViewModelBase
{
    ServerViewModel m_ServerViewModel;
    ServerQueryHelper m_ServerQueryHelper;

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

    #endregion
    #endregion

    #region - commands -
    #region [Execute_Close]
    public void Execute_Close()
    {
        m_ServerViewModel.RemoveQuery(this);
    }
    #endregion

    #region [Execute_ExecuteSQL]
    public void Execute_ExecuteSQL()
    {
        if (DatabaseObject is DatabaseInfo dbInfo)
            FillDataTable(SelectedQueryText.IsNotNullOrEmpty() ? SelectedQueryText : QueryText, dbInfo);
    }
    #endregion

    #region [Execute_ClearResult]
    public void Execute_ClearResult()
    {
        ResultSets.Clear();
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
            ResultSets.Clear();

        int expectedResults = 0;
        int receivedResults = 0;

        var sqls = sql.Split(";").Where(s => s.IsNotNullOrEmpty()).Take(1); //First simple way to separate sqls. search another way with regex
        expectedResults = sqls.Count();

        if (sqls.Any())
            IsLoading = true;

        foreach (var singleSql in sqls)
        {
            new Task(() =>
            {
                var dataTable = m_ServerQueryHelper.FillDataTable(databaseInfo, singleSql.ToStringValue());

                receivedResults++;

                if (expectedResults == receivedResults)
                    __ExecuteOnDispatcher(() => IsLoading = false);

                if (dataTable == null)
                    return;

                __ExecuteOnDispatcher(() =>
                 ResultSets.Add(new ResultSetViewModel()
                 {
                     ResultLines = dataTable.DefaultView
                 }));
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
    #endregion
}
