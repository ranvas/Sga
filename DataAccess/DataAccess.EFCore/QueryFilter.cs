using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore
{
    public class QueryFilter<T>
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public Expression<Func<T, int>> OrderByDescending { get; set; }

        public QueryFilter()
        {

        }

        public QueryFilter(int? skip = null, int? take = null, Expression<Func<T, int>> orderByDescending = null)
        {
            Skip = skip;
            Take = take;
            OrderByDescending = orderByDescending;
        }
    }

    public static class QueryFilterExtensions
    {
        public static IQueryable<T> Apply<T>(this IQueryable<T> query, QueryFilter<T> filter)
        {
            if (filter == null)
                return query;

            if (filter.Skip.HasValue)
                query = query.Skip(filter.Skip.Value);

            if (filter.Take.HasValue)
                query = query.Take(filter.Take.Value);

            if (filter.OrderByDescending != null)
                query = query.OrderByDescending(filter.OrderByDescending);

            return query;
        }
    }
}
