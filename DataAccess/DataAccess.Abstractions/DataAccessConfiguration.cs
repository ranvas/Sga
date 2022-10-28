using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions
{
    public interface IDataAccessConfiguration
    {
        string ConnectionString { get; set; }
    }

    public class DataAccessConfiguration : IDataAccessConfiguration
    {
        public DataAccessConfiguration() : this(string.Empty)
        {
        }

        public DataAccessConfiguration(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }
}
