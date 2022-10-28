using System.Data.Common;

namespace DataAccess.EFCore
{
    public class DbCommandContext
    {
        public DbCommandContext(DbCommand command)
        {
            Command = command;
        }

        public DbCommand Command { get; }
    }
}