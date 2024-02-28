using DBison.Core.Entities;

namespace DBison.Plugin;
public interface IContextMenuPlugin : IPlugin
{
    #region - properties -

    #region [Header]
    /// <summary>
    /// Get the header of the context menu section.
    /// </summary>
    string Header { get; }
    #endregion

    #region [CommandName]
    /// <summary>
    /// Get the command name.
    /// </summary>
    string CommandName { get; }
    #endregion

    #endregion

    #region - methods -

    #region [Execute]
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="databaseInfo">The selected database.</param>
    /// <returns>The operation result.</returns>
    OperationResult Execute(ExtendedDatabaseInfo databaseInfo);
    #endregion

    #endregion
}
