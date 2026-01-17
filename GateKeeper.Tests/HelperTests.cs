using CGPGK.Models;
using CGPGK.Services;
using CGPGK.Utils;
using GateKeeper.API;
using GateKeeper.Core.Application;
using GateKeeper.Core.Context;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Services;
using GateKeeper.Helper.Application;
using GateKeeper.Helper.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using System;
using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace GateKeeper.Tests
{
    public class HelperTests 
    {
        private HelperApplication helperApplication;
        public HelperTests()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"HelperAppSettings.json");
            IConfiguration config = builder.Build();
            var appSettings = config.GetSection("Settings").Get<AppSettings>();
            Assert.NotNull(appSettings);
            FTP.GetInstance(appSettings);
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AppSettings>(appSettings)
                .AddSingleton<WorkerService>()
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
                .AddScoped<AllowedEmailsApplication>()
                .AddScoped<ForeingEmailsApplication>()
                .AddScoped<LocalMonitoredEmailsApplication>()
                .AddScoped<HelperApplication>()
                .BuildServiceProvider();    
            var scope = serviceProvider.CreateScope();
            helperApplication = scope.ServiceProvider.GetRequiredService<HelperApplication>();
        }
        public static IEnumerable<object[]> TestFiles => new List<object[]>
        {
            new object[] { @"HelperTestFiles/NormalEmail.msg", new EmailFields("from@example.com") { To = new HashSet<string> { "a@example.com", "b@example.com" } } },
            new object[] { @"HelperTestFiles/SingleToEmail.msg", new EmailFields("from@example.com") { To = new HashSet<string> { "a@example.com" } } },
            new object[] { @"HelperTestFiles/ReverseOrderToEmail.msg", new EmailFields("from@example.com") { To = new HashSet<string> { "a@example.com", "b@example.com" } } },

        };
        [Theory]
        [MemberData(nameof(TestFiles))]
        public async Task TestEmailFileProcessing(string fileName, EmailFields expected)
        {
            var res = await helperApplication.ParseEmailFile(Path.Combine(Directory.GetCurrentDirectory(),fileName));
            Assert.Equal(expected, res);
        }

        
    }
    
}
