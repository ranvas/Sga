using DataAccess.Abstractions;
using DataAccess.EFCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccess.EFCore
{
    internal class DbDataTransactionScope : DataTransactionScope
    {
        private DbDataContext? _dataContext;
        private bool _useTransaction;
        private IsolationLevel _isolationLevel = DbScopeOptions.Default().IsolationLevel ?? IsolationLevel.ReadCommitted;
        private IDisposable? _transaction;

        public DbDataTransactionScope(bool useTransaction = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _useTransaction = useTransaction;
            _isolationLevel = isolationLevel;
        }

        internal DbDataContext? DataContext => _dataContext;
        internal EventHandler? BeforeDataContextDisposed;

        internal void SetDataContext(DbDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public override void SaveChanges()
        {
            if (_dataContext != null)
            {
                // create transaction on first save
                if (_useTransaction && _transaction == null)
                {
                    _transaction = _dataContext.BeginTransaction(_isolationLevel.GetDbIsolationLevel());
                }

                // if transaction is not set but isolation level is set and is not default
                if (_transaction == null && _isolationLevel != DbScopeOptions.Default().IsolationLevel)
                {
                    _dataContext.RunDbScope(() => _dataContext.SaveChanges(), new DbScopeOptions(_isolationLevel));
                }
                else
                {
                    _dataContext.SaveChanges();
                }
            }
        }

        public override async Task SaveChangesAsync()
        {
            if (_dataContext != null)
            {
                // create transaction on first save
                if (_useTransaction && _transaction == null)
                {
                    _transaction = await _dataContext.BeginTransactionAsync(_isolationLevel.GetDbIsolationLevel());
                }

                // if transaction is not set but isolation level is set and is not default
                if (_transaction == null && _isolationLevel != DbScopeOptions.Default().IsolationLevel)
                {
                    await _dataContext.RunDbScope(() => _dataContext.SaveChangesAsync(), new DbScopeOptions(_isolationLevel));
                }
                else
                {
                    await _dataContext.SaveChangesAsync();
                }
            }
        }

        public override void Commit()
        {
            if (_dataContext != null && _transaction != null)
            {
                _dataContext.CommitTransaction(_transaction);
            }
        }

        public override async Task CommitAsync()
        {
            if (_dataContext != null && _transaction != null)
            {
                await _dataContext.CommitTransactionAsync(_transaction);
            }
        }

        public override void Rollback()
        {
            if (_dataContext != null && _transaction != null)
            {
                _dataContext.RollbackTransaction(_transaction);
            }
        }

        public override async Task RollbackAsync()
        {
            if (_dataContext != null && _transaction != null)
            {
                await _dataContext.RollbackTransactionAsync(_transaction);
            }
        }

        public override void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            BeforeDataContextDisposed?.Invoke(this, new DbScopeEventData());

            if (_dataContext != null)
            {
                _dataContext.Dispose();
                _dataContext = null;
            }
        }
    }
}
