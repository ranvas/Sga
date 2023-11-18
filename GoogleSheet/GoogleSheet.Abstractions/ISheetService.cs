using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheet.Abstractions
{
    public interface ISheetService<T> where T : class, new()
    {
        Task PostItem(T item);
        Task PostItems(List<T> items);
        Task<List<T>> GetAllItemsAsync();
        Task ClearAllItems();
    }
}
