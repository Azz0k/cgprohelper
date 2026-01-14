using GateKeeper.Core.Application;
using GateKeeper.Core.Context;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace GateKeeper.Tests
{
    public class AllowedDomainsApplicationTests
    {
        AllowedDomainsApplication app;
        AddressesDbContext db;
        public AllowedDomainsApplicationTests()
        {
            ;

            var serviceProvider = new ServiceCollection()
                .AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                })
                .AddDbContext<AddressesDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                })
                .AddScoped<DatabaseService>()
                .AddScoped<AllowedDomainsApplication>()
                .BuildServiceProvider();
            app = serviceProvider.GetRequiredService<AllowedDomainsApplication>();
            db = serviceProvider.GetRequiredService<AddressesDbContext>();
            db.Database.Migrate();
        }
        private string GenerateRandomDomain()
        {
            return Guid.NewGuid().ToString();
        }

        [Fact]
        public async Task SyncTable_ShouldWorkCorrectly()
        {
            string newDomain = GenerateRandomDomain();
            var isFoundDomain = await app.IsDomainExists(newDomain);
            Assert.False(isFoundDomain);
            List<string> newDomains = [newDomain];
            await app.SyncTable(newDomains);
            isFoundDomain = await app.IsDomainExists(newDomain);
            Assert.True(isFoundDomain);
            await app.SyncTable([]);
            isFoundDomain = await app.IsDomainExists(newDomain);
            Assert.False(isFoundDomain);
        }
        [Fact]
        public async Task AddDomain_ShouldWorkCorrectly()
        {
            string newDomain = GenerateRandomDomain();
            var isFoundDomain = await app.IsDomainExists(newDomain);
            Assert.False(isFoundDomain);
            await app.AddAsync(newDomain);
            isFoundDomain = await app.IsDomainExists(newDomain);
            Assert.True(isFoundDomain);
        }
    }
}
