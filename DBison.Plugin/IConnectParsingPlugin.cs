using DBison.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.Plugin;
public interface IConnectParsingPlugin : IPlugin
{
    DatabaseInfo? ParseConnectInput(string connectInput);
}
