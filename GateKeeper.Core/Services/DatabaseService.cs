using GateKeeper.Core.Context;
using GateKeeper.Core.Interfaces;
using GateKeeper.Core.Models.ApiModels;
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
            await TruncateWalAsync();
        }
        public async Task TruncateWalAsync()
        {
            await _db.Database.OpenConnectionAsync();
            await _db.Database.ExecuteSqlRawAsync("PRAGMA wal_checkpoint(TRUNCATE);");
            await _db.Database.CloseConnectionAsync();
        }
        public async Task<T> CreateAsync<T>(T entity) where T : class 
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        public async Task BulkInsertAsync<T>(IEnumerable<T> entitys) where T : class
        {
            await _db.Set<T>().AddRangeAsync(entitys);
            await _db.SaveChangesAsync();
        }
        public async Task<bool> UpdateAsync<T>(int id, Action<T> updateAction) where T : class
        {
            var result = await _db.Set<T>().FindAsync(id);
            if (result == null) return false;
            updateAction(result);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync<T>(int id) where T : class
        {
            if (id <= 0) return false;
            var result = await _db.Set<T>().FindAsync(id);
            if (result == null) return false;
            _db.Set<T>().Remove(result);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<T?> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _db.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _db.Set<T>().Where<T>(predicate).ToListAsync();
        }
        public async Task<List<T>> ReadAllAsync<T>() where T: class 
        {
            return  await _db.Set<T>().ToListAsync();
        }
    }
}
