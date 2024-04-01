namespace DBison.Core.Entities;
public class ExtendedDatabaseInfo : DatabaseInfo
{
    #region [ExtendedDatabaseInfo]
    public ExtendedDatabaseInfo(string name, ServerInfo server, DatabaseInfo dataBase) : base(name, server, dataBase)
    {
    }
    #endregion

    #region [Procedures]
    public List<DBisonStoredProcedure> Procedures { get; private set; } = [];
    #endregion

    #region [Tables]
    public List<DBisonTable> Tables { get; private set; } = [];
    #endregion

    #region [Triggers]
    public List<DBisonTrigger> Triggers { get; private set; } = [];
    #endregion

    #region [Views]
    public List<DBisonView> Views { get; private set; } = [];
    #endregion

    #region [DataFileLocation]
    public string DataFileLocation { get; set; }
    #endregion

    #region [DataFileSize]
    public long DataFileSize { get; set; }
    #endregion

    #region [LogFileLocation]
    public string LogFileLocation { get; set; }
    #endregion

    #region [LogFileSize]
    public long LogFileSize { get; set; }
    #endregion

    #region [ExpectedBackupDirectory]
    public string ExpectedBackupDirectory { get; set; }
    #endregion
}
