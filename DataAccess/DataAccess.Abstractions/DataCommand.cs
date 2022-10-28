using DataAccess.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions
{
    public abstract class DataCommand : IDataCommand
    {
        private IList<object> _parameters;

        public DataCommand()
            : this(string.Empty)
        {
        }

        public DataCommand(string commandText) :
            this(commandText, new List<object>())
        {
        }

        public DataCommand(string commandText, params object[] parameters)
        {
            _parameters = new List<object>(parameters);
            CommandText = commandText;
        }

        public string CommandText { get; set; }
        public IList<object> Parameters
        {
            get
            {
                return _parameters;
            }
        }
    }
}
