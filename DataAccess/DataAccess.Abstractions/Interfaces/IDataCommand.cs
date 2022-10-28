using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Interfaces
{
    /// <summary>
    /// Base command type (DB)
    /// </summary>
    public interface IDataCommand
    {
        string CommandText { get; set; }
        IList<object> Parameters { get; }
    }
}
