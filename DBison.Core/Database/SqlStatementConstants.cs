using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.Core.Database;
internal static class SqlStatementConstants
{
    public const string GET_DATABASE_STATE_STATEMENT = "SELECT state isOnline FROM sys.databases WHERE name = @NAME";
}
