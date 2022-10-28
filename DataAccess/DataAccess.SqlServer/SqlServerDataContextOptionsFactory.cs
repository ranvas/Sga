using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SqlServer
{
    internal static class SqlServerDataContextOptionsFactory
    {
        private static object _lock = new object();
        private static IDictionary<string, DbContextOptionsBuilder> _builders = new Dictionary<string, DbContextOptionsBuilder>();

        public static DbContextOptions? GetOptions(string connectionString, Action<SqlServerDbContextOptionsBuilder>? optionsAction = null)
        {
            return GetOptionsBuilder(connectionString, optionsAction)?.Options ?? null;
        }

        private static DbContextOptionsBuilder? GetOptionsBuilder(string connectionString, Action<SqlServerDbContextOptionsBuilder>? optionsAction = null)
        {
            if (!_builders.ContainsKey(connectionString))
            {
                lock (_lock)
                {
                    if (!_builders.ContainsKey(connectionString))
                    {
                        _builders.Add(connectionString, CreateOptionsBuilder(connectionString, optionsAction));
                    }
                }
            }

            if (_builders.TryGetValue(connectionString, out var result))
            {
                return result;
            }

            return null;
        }

        private static DbContextOptionsBuilder CreateOptionsBuilder(string connectionString, Action<SqlServerDbContextOptionsBuilder>? optionsAction = null)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString, optionsAction);
            // Fix for paging in MS SQL 2008: https://stackoverflow.com/questions/53349099/incorrect-syntax-near-offset-invalid-usage-of-the-option-next-in-the-fetch-st/54200998#54200998
            //            .ReplaceService<IQueryTranslationPostprocessorFactory, SqlServer2008QueryTranslationPostprocessorFactory>();
        }
    }
}
