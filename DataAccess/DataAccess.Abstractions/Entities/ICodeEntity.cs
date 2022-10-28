using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Entities
{
    /// <summary>
    /// Интерфейс сущности с кодом
    /// </summary>
    public interface ICodeEntity
    {
        string Code { get; set; }
    }
}
