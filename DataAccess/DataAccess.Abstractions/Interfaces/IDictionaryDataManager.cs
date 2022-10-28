using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Interfaces
{
    public interface IDictionaryDataManager
    {
        T? Get<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes) where T : class;
        IEnumerable<T> GetAll<T>(params Expression<Func<T, object>>[]? includes) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>(params Expression<Func<T, object>>[]? includes) where T : class;
        Task<T?> GetAsync<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params Expression<Func<T, object>>[]? includes) where T : class;
        IEnumerable<T> GetList<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[]? includes) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[]? includes) where T : class;
        T? Get<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params string[]? includes) where T : class;
        IEnumerable<T> GetAll<T>(params string[]? includes) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>(params string[]? includes) where T : class;
        Task<T?> GetAsync<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null, params string[]? includes) where T : class;
        IEnumerable<T> GetList<T>(Expression<Func<T, bool>> where, params string[]? includes) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> where, params string[]? includes) where T : class;
        T? Get<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null) where T : class;
        IEnumerable<T> GetAll<T>() where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task<T?> GetAsync<T>(Expression<Func<T, bool>> where, IEnumerable<OrderByCriteria<T>>? orderBy = null) where T : class;
        IEnumerable<T> GetList<T>(Expression<Func<T, bool>> where) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> where) where T : class;
    }
}
