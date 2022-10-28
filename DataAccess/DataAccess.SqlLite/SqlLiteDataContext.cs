using DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataAccess.SqlLite
{
    public class SqlLiteDataContext : DbDataContext
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        public SqlLiteDataContext(string connectionString, Action<SqliteDbContextOptionsBuilder>? optionsAction = null)
            : this(SqlLiteDataContextOptionsFactory.GetOptions(connectionString, optionsAction))
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="options">DbContextOptions для конфигурирования в Startup</param>
        public SqlLiteDataContext(DbContextOptions? options) : base(options)
        {
        }
    }
}