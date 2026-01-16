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
    public class AllowedEmailsApplicationTests
    {
        AllowedEmailsApplication app;
        AddressesDbContext db;
        public AllowedEmailsApplicationTests()
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
                .AddScoped<AllowedEmailsApplication>()
                .BuildServiceProvider();
            app = serviceProvider.GetRequiredService<AllowedEmailsApplication>();
            db = serviceProvider.GetRequiredService<AddressesDbContext>();
            db.Database.Migrate();
        }
        private string GenerateRandomEmail()
        {
            return $"test@{Guid.NewGuid().ToString()}";
        }

        [Fact]
        public async Task SyncTable_ShouldWorkCorrectly()
        {
            string newEmail = GenerateRandomEmail();
            var isFoundEmail = await app.IsEmailExists(newEmail);
            Assert.False(isFoundEmail);
            HashSet<string> newEmails = [newEmail];
            await app.SyncTable(newEmails);
            isFoundEmail = await app.IsEmailExists(newEmail);
            Assert.True(isFoundEmail);
            await app.SyncTable([]);
            isFoundEmail = await app.IsEmailExists(newEmail);
            Assert.False(isFoundEmail);
        }
    }
}
