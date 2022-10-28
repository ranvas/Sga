using DataAccess.Abstractions.Interfaces;
using DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccess.EFCore.Extensions;
using System.Transactions;

namespace DataAccess.EFCore
{
    public abstract class DbDataManager : DataManager, IDictionaryDataManager, ITransactionDataManager
    {
        private DbDataTransactionScopeManager _dataTransactionScopeManager;

        public DbDataManager(IDataAccessConfiguration configuration) : base(configuration)
        {
            _dataTransactionScopeManager = new DbDataTransactionScopeManager();
        }

        protected abstract Func<DbDataContext> DataContextCreator { get; }

        public IEnumerable<T> GetAll<T>(params Expression<Func<T, object>>[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery(includes);
                return query.ToList();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(params Expression<Func<T, object>>[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery(includes);
                return await query.ToListAsync();
            }
        }

        public IEnumerable<T> GetList<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery(includes)
                    .ApplyWhereClause(where);

                return query.ToList();
            }
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery(includes)
                    .ApplyWhereClause(where);

                return await query.ToListAsync();
            }
        }

        public T? Get<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery(includes)
                    .ApplyWhereClause(where)
                    .ApplyOrderByClause(orderBy);

                return query.FirstOrDefault();
            }
        }

        public async Task<T?> GetAsync<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery(includes)
                    .ApplyWhereClause(where)
                    .ApplyOrderByClause(orderBy);

                return await query.FirstOrDefaultAsync();
            }
        }

        public IEnumerable<T> GetAll<T>(params string[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery<T>(includes);
                return query.ToList();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(params string[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery<T>(includes);
                return await query.ToListAsync();
            }
        }

        public IEnumerable<T> GetList<T>(Expression<Func<T, bool>> where, params string[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery<T>(includes)
                    .ApplyWhereClause(where);

                return query.ToList();
            }
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery<T>(includes)
                    .ApplyWhereClause(where);

                return await query.ToListAsync();
            }
        }

        public T? Get<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params string[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery<T>(includes)
                    .ApplyWhereClause(where)
                    .ApplyOrderByClause(orderBy);

                return query.FirstOrDefault();
            }
        }

        public async Task<T?> GetAsync<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params string[]? includes) where T : class
        {
            using (var db = CreateDataContext())
            {
                var query = db.CreateQuery<T>(includes)
                    .ApplyWhereClause(where)
                    .ApplyOrderByClause(orderBy);

                return await query.FirstOrDefaultAsync();
            }
        }

        public T? Get<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null) where T : class
            => Get<T>(where, orderBy, new string[] { });
        public IEnumerable<T> GetAll<T>() where T : class
            => GetAll<T>(new string[] { });
        public Task<IEnumerable<T>> GetAllAsync<T>() where T : class
            => GetAllAsync<T>(new string[] { });
        public Task<T?> GetAsync<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null) where T : class
            => GetAsync<T>(where, orderBy, new string[] { });
        public IEnumerable<T> GetList<T>(Expression<Func<T, bool>> where) where T : class
            => GetList<T>(where, new string[] { });
        public Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : class
            => GetListAsync<T>(where, new string[] { });

        public virtual IDataTransactionScope CreateDataTransactionScope(bool useTransaction = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var scope = new DbDataTransactionScope(useTransaction, isolationLevel);
            scope.BeforeDataContextDisposed += scope_BeforeDataContextDisposed;

            // save transaction scope in the registry
            _dataTransactionScopeManager.SetScope(scope.TransactionId, scope);

            return scope;
        }

        protected virtual DbDataContext CreateDataContext()
        {
            return DataContextCreator.Invoke();
        }

        protected virtual DbDataContext? GetDataContext(string transactionId)
        {
            DbDataContext? dataContext = null;

            // get context for transaction
            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                // scope does not exists
                if (!_dataTransactionScopeManager.ScopeExists(transactionId))
                {
                    throw new Exception("Data transaction scope not found. Make sure scope exists and you are NOT using different data managers with the same transaction: it's not allowed by design.");
                }

                var dts = _dataTransactionScopeManager.GetScope(transactionId);
                if (dts == null)
                {
                    throw new Exception($"Unable to get data transaction scope");
                }

                // data context hasn't been set for the transaction scope yet -- create and set it
                if (dts.DataContext == null)
                {
                    dts.SetDataContext(CreateDataContext() ?? throw new Exception("can not CreateDataContext"));

                    // get data context from the transaction scope
                    dataContext = dts.DataContext;
                }
            }
            // create context without scope
            else
            {
                dataContext = CreateDataContext();
            }

            return dataContext;
        }

        private void scope_BeforeDataContextDisposed(object? sender, EventArgs e)
        {
            // remove the scope from registry
            if (sender is DbDataTransactionScope dts && !string.IsNullOrWhiteSpace(dts.TransactionId))
            {
                _dataTransactionScopeManager.RemoveScope(dts.TransactionId);
            }
        }
    }
}
