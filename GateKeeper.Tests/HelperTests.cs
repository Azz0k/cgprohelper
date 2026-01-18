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
using GateKeeper.Core.Models.Entities;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace GateKeeper.Tests
{
    public class HelperTests 
    {
        private static List<AllowedEmails> allowedEmails = [];
        private static List<AllowedDomains> allowedDomains = new List<AllowedDomains>();
        private static List<LocalMonitoredEmails> localMonitoredEmails = new List<LocalMonitoredEmails>();
        private static List<ForeingEmails> foreingEmails = new List<ForeingEmails>();
        private HelperApplication helperApplication;
        private AddressesDbContext db;
        public HelperTests()
        {
            GenerateTestData();
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
            db = scope.ServiceProvider.GetRequiredService<AddressesDbContext>();
            db.Database.Migrate();
            SeedData();
        }
        private string GenRandStr()
        {
            return Guid.NewGuid().ToString();
        }
        private void GenerateTestData()
        {
            allowedEmails.Add(new AllowedEmails() { Email = $"AlLoWedEmAil@{GenRandStr()}"});
            localMonitoredEmails.Add(new LocalMonitoredEmails() { Email = "replyAllowed@example.com", IsReplyAllowed = true });
            localMonitoredEmails.Add(new LocalMonitoredEmails() { Email = "replyNOTallowed@example.com", IsReplyAllowed = false });
            allowedDomains.Add(new AllowedDomains("example.com"));
        }
        private void SeedData()
        {

            foreach(var email in allowedEmails)
            {
                db.allowedEmails.Add(email);
                db.SaveChanges();
            }
            foreach(var email in localMonitoredEmails)
            {
                db.localMonitoredAddresses.Add(email);
                db.SaveChanges();
            }
            foreach (var domain in allowedDomains)
            {
                db.allowedDomains.Add(domain);
                db.SaveChanges();
            }
            db.SaveChanges();
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
        public static IEnumerable<object[]> TestMails => new List<object[]>
        {
            new object[] { new EmailFields("from@example.com") { To = new HashSet<string> { "NotMonitoredEmail@example.com", "notmonitoredemail@example.com" } }, true },
            new object[] { new EmailFields("from@example.com") { To = new HashSet<string> { "replyAllowed@example.com", "replyNOTallowed@example.com" } }, true },
            new object[] { new EmailFields("replyNOTallowed@example.com") { To = new HashSet<string> { allowedEmails[0].Email } }, true },
        };
        [Theory]
        [MemberData(nameof(TestMails))]
        public async Task TestEmailCheckProcessing (EmailFields emails, bool expected)
        {
            bool res = await helperApplication.EnsureSendingAllowed(emails);
            Assert.Equal(expected, res);
        }
    }
    
}
