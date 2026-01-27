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
using Newtonsoft.Json.Linq;
using System;
using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;
namespace GateKeeper.Tests
{
    public class CheckAutorized :
    IClassFixture<CustomWebApplicationFactory<APIProgram>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<APIProgram>
            _factory;
        public CheckAutorized(CustomWebApplicationFactory<APIProgram> factory, ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            _testOutputHelper = testOutputHelper;
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
        [Fact]
        public async Task AllowedDomainApi_ShouldAnswer401()
        {
            string apiUri = "/api/AllowedDomains";
            var addRequest = new AddAllowedDomainsRequest() { Domain = [$"test.test"] };
            var response = await _client.PostAsJsonAsync(apiUri, addRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.GetAsync(apiUri);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.DeleteAsync($"{apiUri}/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var updateRequest = new UpdateDomainRequest() { Id = 1, Domain = "" };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task LocalMonitoredEmailsApi_ShouldAnswer401()
        {
            string apiUri = "/api/LocalMonitoredEmails";
            var addRequest = new AddLocalMonitoredEmailsRequest() { Email = "test@test" };
            var response = await _client.PostAsJsonAsync(apiUri, addRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.GetAsync(apiUri);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.DeleteAsync($"{apiUri}/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var updateRequest = new UpdateLocalMonitoredEmailsRequest() { Id = 1, Email = "" };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task ForeignEmailsApi_ShouldAnswer401()
        {
            string apiUri = "/api/ForeingEmails";
            var addRequest = new AddForeingEmailRequest() { Email = "test@test" };
            var response = await _client.PostAsJsonAsync(apiUri, addRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.GetAsync(apiUri);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.DeleteAsync($"{apiUri}/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task UsersApi_ShouldAnswer401()
        {
            string apiUri = "/api/Users";
            var addRequest = new AddUserRequest() { UserName = "userName", FullName = "fullName", IsAdmin = true, Enabled = true, Password = "password" };
            var response = await _client.PostAsJsonAsync(apiUri, addRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.GetAsync(apiUri);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            response = await _client.DeleteAsync($"{apiUri}/1");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var updateRequest = new UpdateUserRequest() { Id = 1, UserName = "Test", FullName = "Test", Enabled = false, IsAdmin = false };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
