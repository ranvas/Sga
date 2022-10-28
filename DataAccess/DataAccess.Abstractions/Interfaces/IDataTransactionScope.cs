using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Interfaces
{
    public interface IDataTransactionScope : IDisposable
    {
        string TransactionId { get; }
        void SaveChanges();
        Task SaveChangesAsync();
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
