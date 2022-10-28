using DataAccess.EFCore.Extensions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccess.EFCore
{
    internal class DbDataContextScope : IDisposable
    {
        private DbDataContext? _dataContext = null;
        private DbScopeOptions? _options = null;
        private TransactionScope? _transactionScope = null;

        public DbDataContextScope(DbDataContext dataContext, DbScopeOptions? options = null)
        {
            _dataContext = dataContext;
            _options = options ?? DbScopeOptions.Default();

            // commented due to ambient transaction error
            //_transactionScope = CreateTransactionScope(options ?? ScopeOptions.Default());
        }

        public void Run(Action action)
        {
            if (NeedTransaction)
            {
                RunTransaction(action);
            }
            else
            {
                action();
            }
        }

        public T? Run<T>(Func<T> action)
        {
            if (NeedTransaction)
            {
                return RunTransaction(action);
            }

            return action();
        }

        public Task Run(Func<Task> action)
        {
            if (NeedTransaction)
            {
                return RunTransactionAsync(action);
            }

            return action();
        }

        public Task<T> Run<T>(Func<Task<T>> action)
        {
            if (NeedTransaction)
            {
                return RunTransactionAsync(action);
            }

            return action();
        }

        public void Dispose()
        {
            if (_transactionScope != null)
            {
                _transactionScope.Complete();
                _transactionScope.Dispose();
            }
        }

        protected virtual TransactionScope CreateTransactionScope(DbScopeOptions options)
        {
            if (options != null && !options.IsEmpty)
            {
                var transactionOptions = new TransactionOptions();

                if (options.IsolationLevel.HasValue)
                {
                    transactionOptions.IsolationLevel = options.IsolationLevel.Value;
                }

                if (options.TimeoutMilliseconds.HasValue)
                {
                    transactionOptions.Timeout = TimeSpan.FromMilliseconds(options.TimeoutMilliseconds.Value);
                }

                return new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            }

            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);   // allow async/await inside the scope
        }

        private bool NeedTransaction => _options != null && _options.IsolationLevel.HasValue;

        private void RunTransaction(Action action)
        {
            using (var tr = BeginTransaction())
            {
                try
                {
                    action();
                    tr.Commit();
                }
                catch
                {
                    tr.Rollback();
                }
            }
        }

        private T? RunTransaction<T>(Func<T> action)
        {
            T? result = default;

            using (var tr = BeginTransaction())
            {
                try
                {
                    result = action();
                    tr.Commit();
                }
                catch
                {
                    tr.Rollback();
                }
            }

            return result;
        }

        private async Task RunTransactionAsync(Func<Task> action)
        {
            using (var tr = await BeginTransactionAsync())
            {
                try
                {
                    await action();
                    tr.Commit();
                }
                catch
                {
                    tr.Rollback();
                }
            }
        }

        private async Task<T> RunTransactionAsync<T>(Func<Task<T>> action)
        {
            T? result;

            using (var tr = await BeginTransactionAsync())
            {
                try
                {
                    result = await action();
                    tr.Commit();
                }
                catch
                {
                    try
                    {
                        tr.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }

            return result;
        }

        private IDbContextTransaction BeginTransaction()
        {
            return _dataContext?.BeginTransaction(GetDbIsolationLevel()) as IDbContextTransaction 
                ?? throw new NullReferenceException("dataContext must be implemented");
        }

        private async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if(_dataContext == null)
                throw new NullReferenceException("dataContext must be implemented");
            var result = await _dataContext.BeginTransactionAsync(GetDbIsolationLevel());
            return result as IDbContextTransaction ?? throw new InvalidCastException("could not cast IDisposable to IDbContextTransaction");
        }

        private System.Data.IsolationLevel GetDbIsolationLevel()
        {
            return (_options?.IsolationLevel ?? DbScopeOptions.Default().IsolationLevel!.Value).GetDbIsolationLevel();
        }
    }
}
