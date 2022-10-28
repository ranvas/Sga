using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SqlLite
{
    internal static class SqlLiteDataContextOptionsFactory
    {
        private static object _lock = new object();
        private static IDictionary<string, DbContextOptionsBuilder> _builders = new Dictionary<string, DbContextOptionsBuilder>();

        public static DbContextOptions? GetOptions(string connectionString, Action<SqliteDbContextOptionsBuilder>? optionsAction = null)
        {
            return GetOptionsBuilder(connectionString, optionsAction)?.Options ?? null;
        }

        private static DbContextOptionsBuilder? GetOptionsBuilder(string connectionString, Action<SqliteDbContextOptionsBuilder>? optionsAction = null)
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

        private static DbContextOptionsBuilder CreateOptionsBuilder(string connectionString, Action<SqliteDbContextOptionsBuilder>? optionsAction = null)
        {
            return SqliteDbContextOptionsBuilderExtensions.UseSqlite(new DbContextOptionsBuilder(), connectionString, optionsAction);
        }
    }
}
