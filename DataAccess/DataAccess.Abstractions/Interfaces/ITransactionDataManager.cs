using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccess.Abstractions.Interfaces
{
    public interface ITransactionDataManager
    {
        IDataTransactionScope CreateDataTransactionScope(bool useTransaction = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
