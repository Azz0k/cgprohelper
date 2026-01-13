using GateKeeper.Core.Models.ApiModels;
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
        Task<T> CreateAsync<T> (T entity) where T :class;
        Task<T?> FindAsync<T> (Expression<Func<T, bool>> predicate) where T : class;
        Task<List<T>> ReadAllAsync<T>() where T : class;
        Task<bool> UpdateAsync<T>(int id, Action<T> updateAction) where T : class;
        Task<bool> DeleteAsync<T>(int id) where T : class;
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class;



    }
}
