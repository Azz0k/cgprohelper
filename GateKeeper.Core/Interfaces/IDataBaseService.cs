using GateKeeper.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GateKeeper.Core.Interfaces
{
    public interface IDataBaseService
    {
        Task InitDatabaseAsync();
        Task CreateAsync<T> (T entity) where T :class;
        Task<bool> ExistsAsync<T> (Expression<Func<T, bool>> predicate) where T : class;
        Task<T> Read<T>(string key);
        Task<List<T>> ReadAllAsync<T>() where T : class;
        Task Update<T>(T entity);
        Task<T> Delete<T>(string key);
        

    }
}
