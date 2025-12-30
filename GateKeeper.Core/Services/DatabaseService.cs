using GateKeeper.Core.Context;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace GateKeeper.Core.Services
{
    public class DatabaseService : IDataBaseService
    {
        private AddressesDbContext _db;
        public DatabaseService(AddressesDbContext db)
        {
            _db = db;
        }
        public async Task InitDatabaseAsync()
        {
            await _db.Database.MigrateAsync();
            await _db.Database.OpenConnectionAsync();
            await _db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");
            await _db.Database.CloseConnectionAsync();

        }
        public async Task CreateAsync<T>(T entity) where T : class 
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }
        public Task<T> Read<T>(string key)
        {
            throw new NotImplementedException();
        }
        public Task Update<T>(T entity)
        {
            throw new NotImplementedException();
        }
        public Task<T> Delete<T>(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _db.Set<T>().AnyAsync(predicate);
        }
        public async Task<List<T>> ReadAllAsync<T>() where T: class 
        {
            return  await _db.Set<T>().ToListAsync();
        }
    }
}
