using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheet.Abstractions
{
    public interface ISheetAdapter
    {
        Task Post(List<IList<object>> data, string spreadSheetId, IGoogleSheetRange range);
        Task<IList<IList<object>>> GetDataFromRange(string spreadSheetId, IGoogleSheetRange range);
        Task ClearItems(string spreadSheetId, IGoogleSheetRange range);
    }
}
