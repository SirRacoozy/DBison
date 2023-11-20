using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.Plugin;
public interface IPlugin
{
    /// <summary>
    /// Get the name of the plugin.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Get the description of the plugin.
    /// </summary>
    string Description { get; }
}
