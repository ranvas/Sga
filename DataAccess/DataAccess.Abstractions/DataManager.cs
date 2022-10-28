using DataAccess.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions
{
    public abstract class DataManager : IDataManager
    {
        public DataManager(IDataAccessConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IDataAccessConfiguration Configuration { get; }
    }
}
