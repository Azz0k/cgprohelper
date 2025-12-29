using GateKeeper.Core.Context;
using GateKeeper.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    }
}
