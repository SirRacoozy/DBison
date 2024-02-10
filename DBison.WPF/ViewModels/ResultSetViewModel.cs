using DBison.Core.Baseclasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.WPF.ViewModels;
public class ResultSetViewModel : ViewModelBase
{
    public ResultSetViewModel()
    {
    }

    public DataView ResultLines
    {
        get => Get<DataView>();
        set => Set(value);
    }
}
