using GateKeeper.API;
using GateKeeper.Core.Context;
using GateKeeper.Core.Models.ApiModels;
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
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace GateKeeper.Tests
{
    public class AllowedDomainsAPITests :
    IClassFixture<CustomWebApplicationFactory<APIProgram>>
    {

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<APIProgram>
            _factory;
        private string apiUri = "/api/AllowedDomains";

        public AllowedDomainsAPITests(
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
        private async Task<List<AllowedDomainsDTO>?> GetAsync(string apiUri)
        {
            var response = await _client.GetAsync(apiUri);
            var content = await response.Content.ReadFromJsonAsync<List<AllowedDomainsDTO>>();
            return content;
        }
        private string GenerateRandomDomainName()
        {
            return Guid.NewGuid().ToString(); 
        }
        private async Task<Dictionary<int, List<AllowedDomainsDTO>>?> PostToDomainAPI()
        {
            // await DoMigrateAsync();
            string d1 = GenerateRandomDomainName();
            string d2 = GenerateRandomDomainName();
            var addDomainRequest = new AddAllowedDomainsRequest() { Domain = [$"{d1}.{d1}", $"{d2}.{d2}", $"{d2}.{d2}", ".", "1,txt"] };
            var response = await _client.PostAsJsonAsync(apiUri, addDomainRequest);
            response.EnsureSuccessStatusCode();
            Dictionary<int, List<AllowedDomainsDTO>>? addedData = await response.Content.ReadFromJsonAsync<Dictionary<int, List<AllowedDomainsDTO>>>();
            return addedData;
        }
        [Fact]
        public async Task DomainApi_Post_ShouldWorkCorrectly()
        {
            Dictionary<int, List<AllowedDomainsDTO>>? addedData = await PostToDomainAPI();
            Assert.NotNull(addedData);
            Assert.Equal(2, addedData.Count);
            Assert.Equal(2, addedData[201].Count);
            foreach (var item in addedData[201])
            {
                Assert.NotEqual(0, item.Id);
            }
            Assert.Equal(2, addedData[400].Count);
        }
        [Fact]
        public async Task DomainApi_Get_ShouldWorkCorrectly()
        {
            Dictionary<int, List<AllowedDomainsDTO>>? addedData = await PostToDomainAPI();
            List<AllowedDomainsDTO>? domains = await GetAsync(apiUri);
            Assert.NotNull(domains);
            Assert.NotEmpty(domains);
        }
        [Fact]
        public async Task DomainApi_Delete_ShouldWorkCorrectly()
        {
            Dictionary<int, List<AllowedDomainsDTO>>? addedData = await PostToDomainAPI();
            List<AllowedDomainsDTO>? domains = await GetAsync(apiUri);
            Assert.NotNull(domains);
            int idToDelete = domains.First().Id;
            var response = await _client.DeleteAsync($"{apiUri}/{idToDelete}");
            response.EnsureSuccessStatusCode();
            response = await _client.DeleteAsync($"{apiUri}/{idToDelete}");
            var code = response.StatusCode;
            Assert.Equal(HttpStatusCode.NotFound, code);
            var domainsAfterDelete = await GetAsync(apiUri);
            Assert.NotNull(domainsAfterDelete);
            Assert.DoesNotContain(domainsAfterDelete, e=> e.Id==idToDelete);
        }
        [Fact]
        public async Task DomainApi_Put_ShouldWorkCorrectly()
        {
            Dictionary<int, List<AllowedDomainsDTO>>? addedData = await PostToDomainAPI();
            List<AllowedDomainsDTO>? domains = await GetAsync(apiUri);
            Assert.NotNull(domains);
            var updateRequest = new UpdateDomainRequest() { Id = domains.First().Id, Domain = "update.update" };
            var response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            response.EnsureSuccessStatusCode();
            domains = await GetAsync(apiUri);
            Assert.NotNull(domains);
            Assert.Equal(updateRequest.Domain, domains[0].Domain);
            updateRequest = new UpdateDomainRequest() { Id = domains.First().Id, Domain = "update." };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            var code = response.StatusCode;
            Assert.Equal(HttpStatusCode.BadRequest, code);
            updateRequest = new UpdateDomainRequest() { Id = -1, Domain = "update.update" };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            code = response.StatusCode;
            Assert.Equal(HttpStatusCode.NotFound, code);
        }
    }
}