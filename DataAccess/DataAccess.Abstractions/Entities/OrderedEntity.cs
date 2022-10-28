using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Entities
{
    /// <summary>
    /// Упорядоченная сущность. Имеет специальное поле - порядок при печати
    /// </summary>
    [DataContract]
    public class AbstractOrderedEntity : AbstractNamedEntity
    {
        [DataMember]
        public virtual int? PrintOrder { get; set; }
    }

    [DataContract]
    public class OrderedEntity : AbstractOrderedEntity
    {
    }
}
