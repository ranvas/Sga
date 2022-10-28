using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccess.EFCore.Extensions
{
    public static class IsolationLevelExtensions
    {
        public static System.Data.IsolationLevel GetDbIsolationLevel(this IsolationLevel isolationLevel)
        {
            switch (isolationLevel)
            {
                case IsolationLevel.ReadUncommitted:
                    return System.Data.IsolationLevel.ReadUncommitted;

                case IsolationLevel.ReadCommitted:
                    return System.Data.IsolationLevel.ReadCommitted;

                case IsolationLevel.RepeatableRead:
                    return System.Data.IsolationLevel.RepeatableRead;

                case IsolationLevel.Serializable:
                    return System.Data.IsolationLevel.Serializable;

                case IsolationLevel.Snapshot:
                    return System.Data.IsolationLevel.Snapshot;

                default:
                    return System.Data.IsolationLevel.ReadCommitted;    // TODO: change to snapshot
            }
        }
    }
}
