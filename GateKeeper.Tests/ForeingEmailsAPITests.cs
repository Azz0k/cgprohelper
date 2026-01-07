using GateKeeper.API;
using GateKeeper.Core.Application;
using GateKeeper.Core.Context;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using static GateKeeper.Core.Utils.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace GateKeeper.Tests
{
    public class ForeingEmailsAPITests :
    IClassFixture<CustomWebApplicationFactory<APIProgram>>
    {

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<APIProgram>
            _factory;
        private string apiUri = "/api/ForeingEmails";
        private string depprecatedEmail = "test@deprecated.date";

        public ForeingEmailsAPITests(
            CustomWebApplicationFactory<APIProgram> factory, ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            DoMigrate();
        }
        private void DoMigrate()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AddressesDbContext>();
                db.Database.Migrate();
            }
        }
        private async Task SeedDeprecatedDate()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AddressesDbContext>();
                ForeingEmails newEmail = new ForeingEmails() { Email = depprecatedEmail, ReceivedDate = GenerateDeprecatedDate() };
                await db.Set<ForeingEmails>().AddAsync(newEmail);
                await db.SaveChangesAsync();
            }
        }
        private async Task CleanDeprecatedDate()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var app = scopedServices.GetRequiredService<ForeingEmailsApplication>();
                await app.RemoveDeprecatedRecords();
            }
        }
        private async Task<List<ForeingEmailsDTO>?> GetAsync()
        {
            var response = await _client.GetAsync(apiUri);
            var content = await response.Content.ReadFromJsonAsync<List<ForeingEmailsDTO>>();
            return content;
        }
        private string GenerateRandomDomainName()
        {
            return Guid.NewGuid().ToString();
        }
        private async Task<string?> PostToForeignEmailsAPI()
        {
            string domain = GenerateRandomDomainName();
            AddForeingEmailRequest request = new() { Email = $"test@{domain}" };
            var response = await _client.PostAsJsonAsync(apiUri, request);
            response.EnsureSuccessStatusCode();
            string? addedData = await response.Content.ReadAsStringAsync();
            return addedData;
        }
        [Fact]
        public async Task ForeignEmailsApi_POST_ShouldWorkCorrectly()
        {
            string? createdId = await PostToForeignEmailsAPI();
            Assert.NotNull(createdId);  
            Assert.NotEmpty(createdId);
        }
        [Fact]
        public async Task ForeignEmailsApi_GET_ShouldWorkCorrectly()
        {
            await PostToForeignEmailsAPI();
            List<ForeingEmailsDTO>? res = await GetAsync();
            Assert.NotNull(res);
            Assert.NotEmpty(res);
            Assert.Equal(GenerateReceivedDate(), res[0].ReceivedDate);
        }
        [Fact]
        public async Task ForeignEmailsApi_DELETE_ShouldWorkCorrectly()
        {
            string? createdId = await PostToForeignEmailsAPI();
            List<ForeingEmailsDTO>? res = await GetAsync();
            Assert.NotNull(res);
            int idToDelete = res[0].Id;
            var response = await _client.DeleteAsync($"{apiUri}/{idToDelete}");
            response.EnsureSuccessStatusCode();
            response = await _client.DeleteAsync($"{apiUri}/{idToDelete}");
            var code = response.StatusCode;
            Assert.Equal(HttpStatusCode.NotFound, code);
            var domainsAfterDelete = await GetAsync();
            Assert.NotNull(domainsAfterDelete);
            Assert.DoesNotContain(domainsAfterDelete, e => e.Id == idToDelete);
        }
        [Fact]
        public async Task ForeignEmailsApplication_RemoveDeprecatedRecords_ShouldWorkCorrectly()
        {
            await SeedDeprecatedDate();
            List<ForeingEmailsDTO>? res = await GetAsync();
            Assert.NotNull(res);
            Assert.Contains(res, e => e.Email == depprecatedEmail);
            await CleanDeprecatedDate();
            res = await GetAsync();
            Assert.NotNull(res);
            Assert.DoesNotContain(res, e => e.Email == depprecatedEmail);
        }
    }
}
