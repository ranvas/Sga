using DataAccess.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataAccess.SqlServer
{
    public class SqlServerDataContext : DbDataContext
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        public SqlServerDataContext(string connectionString, Action<SqlServerDbContextOptionsBuilder>? optionsAction = null)
            : this(SqlServerDataContextOptionsFactory.GetOptions(connectionString, optionsAction))
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="options">DbContextOptions для конфигурирования в Startup</param>
        public SqlServerDataContext(DbContextOptions? options) : base(options)
        {
        }
    }
}