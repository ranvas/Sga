using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions
{
    public class OrderByCriteria<T> where T : class
    {
        public Expression<Func<T, object>>? Term { get; set; }
        public bool IsDesc { get; set; }
    }
}
