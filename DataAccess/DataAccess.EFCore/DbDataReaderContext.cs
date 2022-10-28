using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore
{
    public class DbDataReaderContext : DbCommandContext
    {
        public DbDataReaderContext(DbCommand command, DbDataReader dataReader) : base(command)
        {
            DataReader = dataReader;
        }

        public DbDataReader DataReader { get; }
    }
}
