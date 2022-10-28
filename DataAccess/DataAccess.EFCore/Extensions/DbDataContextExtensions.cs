using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Extensions
{
    public static class DbDataContextExtensions
    {
        public static void RunDbScope(this DbDataContext dataContext, Action action, DbScopeOptions? options = null)
        {
            using (var scope = dataContext.CreateDbScope(options))
            {
                scope.Run(action);
            }
        }

        public static void RunDbReadUncommitted(this DbDataContext dataContext, Action action) => dataContext.RunDbScope(action, DbScopeOptions.ReadUncommitted());

        public static T RunDbScope<T>(this DbDataContext dataContext, Func<T> action, DbScopeOptions? options = null)
        {
            using (var scope = dataContext.CreateDbScope(options))
            {
                return scope.Run(action);
            }
        }

        public static T RunDbReadUncommitted<T>(this DbDataContext dataContext, Func<T> action) => dataContext.RunDbScope(action, DbScopeOptions.ReadUncommitted());

        public static async Task RunDbScopeAsync(this DbDataContext dataContext, Func<Task> action, DbScopeOptions? options = null)
        {
            using (var scope = dataContext.CreateDbScope(options))
            {
                await scope.Run(action);
            }
        }

        public static Task RunDbReadUncommittedAsync(this DbDataContext dataContext, Func<Task> action) => dataContext.RunDbScopeAsync(action, DbScopeOptions.ReadUncommitted());

        public static async Task<T?> RunDbScopeAsync<T>(this DbDataContext dataContext, Func<Task<T>> action, DbScopeOptions? options = null)
        {
            T? result = default;

            using (var scope = dataContext.CreateDbScope(options))
            {
                result = await scope.Run(action);
            }

            return result;
        }

        public static Task<T?> RunDbReadUncommittedAsync<T>(this DbDataContext dataContext, Func<Task<T>> action) => dataContext.RunDbScopeAsync<T>(action, DbScopeOptions.ReadUncommitted());


        internal static IQueryable<T> CreateQuery<T>(this DbDataContext db, params Expression<Func<T, object>>[]? includes) where T : class
        {
            var query = db.Query<T>();

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    if (include == null)
                        continue;
                    query = query.Include(include);
                }
            }

            return query;
        }

        internal static IQueryable<T> CreateQuery<T>(this DbDataContext db, params string[]? includes) where T : class
        {
            var query = db.Query<T>();

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        private static IQueryable<T> Query<T>(this DbDataContext db) where T : class
        {
            return db.Set<T>().AsNoTracking().AsQueryable();
        }
    }
}
