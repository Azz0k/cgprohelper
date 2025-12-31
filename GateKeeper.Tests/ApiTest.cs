using GateKeeper.API;
using GateKeeper.Core.Context;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using System;
using System.Data.Common;
using System.Net.Http.Json;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;
namespace GateKeeper.Tests
{

    public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(IDbContextOptionsConfiguration<AddressesDbContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<AddressesDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
            });

            builder.UseEnvironment("Development");
        }
    }

    public class ApiTest :
    IClassFixture<CustomWebApplicationFactory<Program>>
    {

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program>
            _factory;

        public ApiTest(
            CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            
        }
        private async Task DoMigrateAsync()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AddressesDbContext>();
                await db.Database.MigrateAsync();
            }
        }
        [Fact]
        public async Task DomainApi_Get_ShouldReturnDataAfterCertainTranformations()
        {
            string apiUri = "/api/domain";
            await DoMigrateAsync();
            List<string> domains = ["test.test","text.text"];
            var addJson = new AddDomainRequest() { Domain = domains};
            var res = await _client.PostAsJsonAsync(apiUri, addJson);
            var response = await _client.GetAsync(apiUri);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("[{\"id\":1,\"domain\":\"test.test\"},{\"id\":2,\"domain\":\"text.text\"}]",  content);
            res = await _client.DeleteAsync($"{apiUri}/1");
            response = await _client.GetAsync(apiUri);
            content = await response.Content.ReadAsStringAsync();
            Assert.Contains("[{\"id\":2,\"domain\":\"text.text\"}]", content);
        }


    }
}