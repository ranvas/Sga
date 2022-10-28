using DataAccess.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions
{
    public abstract class DataTransactionScope : IDataTransactionScope
    {
        public virtual string TransactionId { get; } = Guid.NewGuid().ToString();

        public abstract void SaveChanges();
        public abstract Task SaveChangesAsync();
        public abstract void Commit();
        public abstract Task CommitAsync();
        public abstract void Rollback();
        public abstract Task RollbackAsync();

        public virtual void Dispose()
        {
            // Do nothing
        }
    }
}
