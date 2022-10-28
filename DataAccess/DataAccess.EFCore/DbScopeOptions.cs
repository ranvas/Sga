using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccess.EFCore
{
    /// <summary>
    /// Unit of work scope settings
    /// </summary>
    public class DbScopeOptions
    {
        public IsolationLevel? IsolationLevel { get; set; }
        public ulong? TimeoutMilliseconds { get; set; }

        public DbScopeOptions(IsolationLevel? isolationLevel = null, ulong? timeoutMilliseconds = null)
        {
            IsolationLevel = isolationLevel;
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public bool IsEmpty
        {
            get
            {
                return !this.IsolationLevel.HasValue && !this.TimeoutMilliseconds.HasValue;
            }
        }

        public static DbScopeOptions ReadUncommitted(ulong? timeoutMilliseconds = null)
        {
            return new DbScopeOptions()
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                TimeoutMilliseconds = timeoutMilliseconds
            };
        }

        public static DbScopeOptions Default(ulong? timeoutMilliseconds = null)
        {
            return new DbScopeOptions()
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,  // TODO: change to snapshot
                TimeoutMilliseconds = timeoutMilliseconds
            };
        }
    }
}
