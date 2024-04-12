using DBison.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.Core.Extender;
public static class OperationResultExtender
{
    #region [Validate]
    public static bool Validate(this OperationResult value)
        => value != null && value.Succeeded;
    #endregion
}
