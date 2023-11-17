using DBison.Core.Entities;
using DBison.Core.Helper;
using DBison.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.ExamplePlugin;
public class MyExampleContextMenuPlugin : IContextMenuPlugin
{
    public string Header => throw new NotImplementedException();

    public string CommandName => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public OperationResult Execute(ExtendedDatabaseInfo databaseInfo) => OperationResult.Ok(nameof(MyExampleContextMenuPlugin));
}
