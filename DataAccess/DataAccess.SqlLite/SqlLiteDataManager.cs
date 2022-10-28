using DataAccess.Abstractions;
using DataAccess.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SqlLite
{
    public abstract class SqlLiteDataManager<TContext> : DbDataManager where TContext : SqlLiteDataContext
    {
        public SqlLiteDataManager(IDataAccessConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Creates new DataContext. <b>MUST BE</b> used with USING-clause.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        protected new TContext CreateDataContext()
        {
            return base.CreateDataContext() as TContext ?? throw new InvalidCastException("could not cast DbDataContext to TContext");
        }

        /// <summary>
        /// Returns DataContext for the transaction (creates the new one if does not exist).
        /// <b>ATTENTION! DON'T</b> use it with using-clause. It is shared context which will be disposed inside DbDataTransactionScope.Dispose.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        protected new TContext GetDataContext(string transactionId)
        {
            return base.GetDataContext(transactionId) as TContext ?? throw new InvalidCastException("could not cast DbDataContext to TContext");
        }
    }
}
