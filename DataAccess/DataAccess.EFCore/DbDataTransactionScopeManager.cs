using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore
{
    internal class DbDataTransactionScopeManager
    {
        private ConcurrentDictionary<string, DbDataTransactionScope> _transactionScopes;

        public DbDataTransactionScopeManager()
        {
            _transactionScopes = new ConcurrentDictionary<string, DbDataTransactionScope>();
        }

        internal bool ScopeExists(string transactionId)
        {
            return _transactionScopes.ContainsKey(transactionId);
        }

        internal DbDataTransactionScope GetScope(string transactionId)
        {
            _transactionScopes.TryGetValue(transactionId, out var dts);
            return dts;
        }

        internal void SetScope(string transactionId, DbDataTransactionScope scope)
        {
            _transactionScopes.AddOrUpdate(transactionId, scope, (k, v) => scope);
        }

        internal void RemoveScope(string transactionId)
        {
            _transactionScopes.TryRemove(transactionId, out var _);
        }
    }
}
