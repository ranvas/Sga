using DataAccess.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.EFCore.Extensions;

namespace DataAccess.EFCore
{
    /// <summary>
    /// Base class for MS SQL DataSource
    /// </summary>
    // !! ВАЖНО !!
    // IDbModelCacheKeyProvider позволяет задать ключ, по которому EF кэширует Model.
    // В данном случае используется название схемы пользователя (его логин).
    // Если этого не сделать, то схема будет кэшироваться только один раз - только для того пользователя, который вошел первым.
    // Если первым вошел MEGATEC, то все вызовы, имеющие непустое значение SchemaName будут выполняться со схемой MEGATEC (например, MEGATEC.Dogovor).
    // Реализация интерфейса позволяет сказать EF, что Model надо кэшировать для каждого значения SchemaName.
    public abstract class DbDataContext : DbContext, IDataContext//, IDbModelCacheKeyProvider
    {
        // TODO: discover multi-tenant (IDbModelCacheKeyProvider in EF 6)
        //protected string SchemaName { get; set; }

        public DbDataContext()
        {
            Init();
        }

        public DbDataContext(DbContextOptions? options) : base(options ?? throw new NotImplementedException("Default options not implemented"))
        {
            Init();
        }

        private void Init()
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            this.SavingChanges += DataContext_SavingChanges;
        }

        private static void DataContext_SavingChanges(object sender, SavingChangesEventArgs e)
        {
            var context = sender as DbContext;

            // set identity column values to default (because it is necessary in EF Core, otherwise we receive SET IDENTITY_INSERT error)
            PrepareIdentitySave(context);
        }

        internal DbDataContextScope CreateDbScope(DbScopeOptions? options = null)
        {
            return new DbDataContextScope(this, options);
        }

        public async Task<int> ExecuteNonQueryAsync(IDataCommand dataCommand, Action<DbCommandContext> onExecuted = null)
        {
            using (var cmd = CreateCommand(dataCommand))
            {
                bool wasOpen = await OpenConnectionAsync(cmd);

                try
                {
                    var result = await cmd.ExecuteNonQueryAsync();

                    onExecuted?.Invoke(new DbCommandContext(cmd));

                    return result;
                }
                finally
                {
                    await CloseConnectionAsync(cmd, wasOpen);
                }
            }
        }

        public int ExecuteNonQuery(IDataCommand dataCommand, Action<DbCommandContext> onExecuted = null)
        {
            using (var cmd = CreateCommand(dataCommand))
            {
                bool wasOpen = OpenConnection(cmd);

                try
                {
                    var result = cmd.ExecuteNonQuery();

                    onExecuted?.Invoke(new DbCommandContext(cmd));

                    return result;
                }
                finally
                {
                    CloseConnection(cmd, wasOpen);
                }
            }
        }

        public async Task ExecuteReaderAsync(IDataCommand dataCommand, Action<DbDataReaderContext> onExecuted = null)
        {
            using (var cmd = CreateCommand(dataCommand))
            {
                bool wasOpen = await OpenConnectionAsync(cmd);

                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        try
                        {
                            onExecuted?.Invoke(new DbDataReaderContext(cmd, reader));
                        }
                        finally
                        {
                            if (!reader.IsClosed)
                            {
                                await reader.CloseAsync();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw;
                }
                finally
                {
                    await CloseConnectionAsync(cmd, wasOpen);
                }
            }
        }

        public void ExecuteReader(IDataCommand dataCommand, Action<DbDataReaderContext>? onExecuted = null)
        {
            using (var cmd = CreateCommand(dataCommand))
            {
                bool wasOpen = OpenConnection(cmd);

                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            onExecuted?.Invoke(new DbDataReaderContext(cmd, reader));
                        }
                        finally
                        {
                            if (!reader.IsClosed)
                            {
                                reader.Close();
                            }
                        }
                    }
                }
                finally
                {
                    CloseConnection(cmd, wasOpen);
                }
            }
        }


        public async Task<IEnumerable<T>?> ExecuteQueryAsync<T>(IDataCommand dataCommand, Action<DbCommandContext> onExecuted = null) where T : new()
        {
            // Only DbCommandContext is allowed on onExecuted, because we use reader. So, only command can be accessed.

            IEnumerable<T>? result = null;

            await ExecuteReaderAsync(dataCommand, (ctx) =>
            {
                result = ctx.DataReader.Translate<T>();

                onExecuted?.Invoke(new DbCommandContext(ctx.Command));
            });

            return result;
        }

        public IEnumerable<T>? ExecuteQuery<T>(IDataCommand dataCommand, Action<DbCommandContext>? onExecuted = null) where T : new()
        {
            // Only DbCommandContext is allowed on onExecuted, because we use reader. So, only command can be accessed.

            IEnumerable<T>? result = null;

            ExecuteReader(dataCommand, (ctx) =>
            {
                result = ctx.DataReader.Translate<T>();

                onExecuted?.Invoke(new DbCommandContext(ctx.Command));
            });

            return result;
        }

        public async Task<IEnumerable<T>?> ExecuteQueryValueAsync<T>(IDataCommand dataCommand, Action<DbCommandContext>? onExecuted = null) where T : struct
        {
            // Only DbCommandContext is allowed on onExecuted, because we use reader. So, only command can be accessed.

            IEnumerable<T>? result = null;

            await ExecuteReaderAsync(dataCommand, (ctx) =>
            {
                result = ctx.DataReader.TranslateValue<T>();

                onExecuted?.Invoke(new DbCommandContext(ctx.Command));
            });

            return result;
        }

        public IEnumerable<T>? ExecuteQueryValue<T>(IDataCommand dataCommand, Action<DbCommandContext>? onExecuted = null) where T : struct
        {
            // Only DbCommandContext is allowed on onExecuted, because we use reader. So, only command can be accessed.

            IEnumerable<T>? result = null;

            ExecuteReader(dataCommand, (ctx) =>
            {
                result = ctx.DataReader.TranslateValue<T>();

                onExecuted?.Invoke(new DbCommandContext(ctx.Command));
            });

            return result;
        }

        public async Task<T> ExecuteScalarAsync<T>(IDataCommand dataCommand)
        {
            // From: https://stackoverflow.com/questions/46163254/how-to-get-scalar-value-from-a-sql-statement-in-a-net-core-application
            // https://entityframeworkcore.com/knowledge-base/57645042/-net-core-ef--cleaning-up-sqlconnection-createcommand

            using (var cmd = CreateCommand(dataCommand))
            {
                bool wasOpen = await OpenConnectionAsync(cmd);

                try
                {
                    return (T)await cmd.ExecuteScalarAsync();
                }
                finally
                {
                    await CloseConnectionAsync(cmd, wasOpen);
                }
            }
        }

        public T ExecuteScalar<T>(IDataCommand dataCommand)
        {
            using (var cmd = CreateCommand(dataCommand))
            {
                bool wasOpen = OpenConnection(cmd);

                try
                {
                    return (T)cmd.ExecuteScalar();
                }
                finally
                {
                    CloseConnection(cmd, wasOpen);
                }
            }
        }

        public IDisposable BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            // https://stackoverflow.com/questions/40611874/use-the-same-transaction-in-different-methods-with-entity-framework-core
            // https://docs.microsoft.com/en-us/ef/core/saving/transactions

            return Database.BeginTransaction(isolationLevel);
        }

        public async Task<IDisposable> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel)
        {
            return await Database.BeginTransactionAsync(isolationLevel);
        }


        public void CommitTransaction(IDisposable transaction)
        {
            if (!(transaction is IDbContextTransaction tran))
            {
                throw new InvalidCastException(string.Format("Cannot commit transaction. Unable to cast {0} to DbContextTransaction", transaction == null ? "NULL" : transaction.GetType().ToString()));
            }

            tran.Commit();
        }

        public Task CommitTransactionAsync(IDisposable transaction)
        {
            if (!(transaction is IDbContextTransaction tran))
            {
                throw new InvalidCastException(string.Format("Cannot commit transaction. Unable to cast {0} to DbContextTransaction", transaction == null ? "NULL" : transaction.GetType().ToString()));
            }

            return tran.CommitAsync();
        }

        public void RollbackTransaction(IDisposable transaction)
        {
            if (!(transaction is IDbContextTransaction tran))
            {
                throw new InvalidCastException(string.Format("Cannot rollback transaction. Unable to cast {0} to DbContextTransaction", transaction == null ? "NULL" : transaction.GetType().ToString()));
            }

            tran.Rollback();
        }

        public Task RollbackTransactionAsync(IDisposable transaction)
        {
            if (!(transaction is IDbContextTransaction tran))
            {
                throw new InvalidCastException(string.Format("Cannot rollback transaction. Unable to cast {0} to DbContextTransaction", transaction == null ? "NULL" : transaction.GetType().ToString()));
            }

            return tran.RollbackAsync();
        }

        public override int SaveChanges()
        {
            this.ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ChangeTracker.DetectChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        private DbCommand CreateCommand(IDataCommand dataCommand = null)
        {
            var command = this.Database.GetDbConnection().CreateCommand();

            // check current transaction
            // https://stackoverflow.com/questions/52352636/pass-current-transaction-to-dbcommand
            if (this.Database.CurrentTransaction != null)
            {
                command.Transaction = this.Database.CurrentTransaction.GetDbTransaction();
            }

            if (dataCommand != null)
            {
                command.CommandText = dataCommand.CommandText;
                command.CommandTimeout = 600; // 10 minutes

                if (dataCommand.Parameters != null && dataCommand.Parameters.Any())
                {
                    command.Parameters.AddRange(dataCommand.Parameters.ToArray());
                }
            }

            return command;
        }

        private bool OpenConnection(DbCommand cmd)
        {
            bool wasOpen = cmd.Connection.State == System.Data.ConnectionState.Open;

            if (!wasOpen)
            {
                cmd.Connection.Open();
            }

            return wasOpen;
        }

        private async Task<bool> OpenConnectionAsync(DbCommand cmd)
        {
            bool wasOpen = cmd.Connection.State == System.Data.ConnectionState.Open;

            if (!wasOpen)
            {
                await cmd.Connection.OpenAsync();
            }

            return wasOpen;
        }


        private void CloseConnection(DbCommand cmd, bool wasOpen)
        {
            if (!wasOpen)
            {
                cmd.Connection.Close();
            }
        }

        private async Task CloseConnectionAsync(DbCommand cmd, bool wasOpen)
        {
            if (!wasOpen)
            {
                await cmd.Connection.CloseAsync();
            }
        }

        /// <summary>
        /// Sets identity column values to default (because it is necessary in EF Core, otherwise we receive SET IDENTITY_INSERT error)
        /// </summary>
        /// <param name="context"></param>
        private static void PrepareIdentitySave(DbContext context)
        {
            var entries = context.ChangeTracker.Entries();

            foreach (var en in entries)
            {
                if (en.State == EntityState.Added)
                {
                    if (en.HasIdentityColumns())
                    {
                        foreach (var p in en.Properties)
                        {
                            if (p.Metadata.ValueGenerated == ValueGenerated.OnAdd)
                            {
                                var pi = p.Metadata.PropertyInfo;
                                var type = pi.PropertyType;

                                if (type.IsValueType)
                                {
                                    pi.SetValue(en.Entity, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        //public IQueryable<T> QueryCommand<T>(IDataCommand command) where T : class
        //{
        //    //return Database.SqlQuery<T>(command.CommandText, command.Parameters.ToArray()).AsQueryable<T>();

        //    // see for possible solutions
        //    // https://stackoverflow.com/questions/35631903/raw-sql-query-without-dbset-entity-framework-core

        //    return this.Set<T>().FromSqlRaw(command.CommandText, command.Parameters);
        //}
    }
}
