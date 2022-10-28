using DataAccess.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Extensions
{
    internal static class QueryExtensions
    {
        internal static IQueryable<T> ApplyWhereClause<T>(this IQueryable<T> query, Expression<Func<T, bool>>? where) where T : class
        {
            if (where != null)
            {
                query = query.Where(where);
            }

            return query;
        }

        internal static IQueryable<T> ApplyOrderByClause<T>(this IQueryable<T> query, IEnumerable<OrderByCriteria<T>>? orderBy) where T : class
        {
            if (orderBy != null && orderBy.Any())
            {
                int i = 0;
                foreach (var criteria in orderBy)
                {
                    if (criteria.Term == null)
                        continue;
                    if (i == 0)
                    {
                        if (criteria.IsDesc)
                        {
                            query = query.OrderByDescending(criteria.Term);
                        }
                        else
                        {
                            query = query.OrderBy(criteria.Term);
                        }
                    }
                    else
                    {
                        if (query is IOrderedQueryable<T>)
                        {
                            if (criteria.IsDesc)
                            {
                                query = ((IOrderedQueryable<T>)query).ThenByDescending(criteria.Term);
                            }
                            else
                            {
                                query = ((IOrderedQueryable<T>)query).ThenBy(criteria.Term);
                            }
                        }

                    }
                }
            }

            return query;
        }
    }
}
