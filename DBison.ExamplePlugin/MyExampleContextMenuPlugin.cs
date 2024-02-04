using DBison.Core.Entities;
using DBison.Plugin;
using ExtendedDatabaseInfo = DBison.Core.Entities.ExtendedDatabaseInfo;

namespace DBison.ExamplePlugin;
public class MyExampleContextMenuPlugin : IContextMenuPlugin
{
    public string Header => throw new NotImplementedException();

    public string CommandName => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public OperationResult Execute(ExtendedDatabaseInfo databaseInfo) => OperationResult.Ok(nameof(MyExampleContextMenuPlugin));
}
