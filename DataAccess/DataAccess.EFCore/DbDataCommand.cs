using DataAccess.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore
{
    public class DbDataCommand : DataCommand
    {
        public DbDataCommand()
        {
        }

        public DbDataCommand(string commandText) : base(commandText)
        {
        }


        /// <summary>
        /// Создает таблицу ключей для передачи в процедуру на сервер
        /// </summary>
        /// <param name="ids">Коллекция ключей</param>
        /// <returns>Таблица ключей</returns>
        protected DataTable CreateIdTable(IEnumerable<int> ids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("id", typeof(int)));

            foreach (int id in ids)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);

                dr["id"] = id;
            }

            return dt;
        }
    }
}
