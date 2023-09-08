using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonExcel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CsvColumnAttribute : Attribute
    {
        public int Column { get; set; }
        public string Header { get; set; }
        public CsvColumnAttribute(int column, string header)
        {
            Column = column;
            Header = header;
        }
    }
}
