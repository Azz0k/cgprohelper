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
    public class UserApiTests :
        IClassFixture<CustomWebApplicationFactory<APIProgram>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<APIProgram> _factory;
        private string apiUri = "/api/Users";
        public UserApiTests(
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
                var authApp = scopedServices.GetRequiredService<UserAuthenticationApplication>();
                string token = authApp.GenerateJwt(new Core.Models.Entities.User() { FullName = "test", UserName = "test", Id = 1 });
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
        }
        private string GenerateRandomStr()
        {
            return Guid.NewGuid().ToString();
        }
        private async Task<string?> PostToUsersAPI(AddUserRequest addUserRequest)
        {
            return await PostToUsersAPI(addUserRequest.UserName, addUserRequest.FullName, addUserRequest.Password, addUserRequest.IsAdmin, addUserRequest.Enabled);
        }
        private async Task<string?> PostToUsersAPI(string userName, string fullName, string password, bool isAdmin, bool isEnabled)
        {
            AddUserRequest request = new() { UserName = userName, FullName = fullName, IsAdmin = isAdmin, Enabled = isEnabled, Password = password};
            var response = await _client.PostAsJsonAsync(apiUri, request);
            response.EnsureSuccessStatusCode();
            string? addedData = await response.Content.ReadAsStringAsync();
            return addedData;
        }
        private async Task<List<UserDTO>?> GetAsync()
        {
            var response = await _client.GetAsync(apiUri);
            var content = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
            return content;
        }
        [Fact]
        public async Task UsersApi_POST_ShouldWorkCorrectly()
        {
            string? createdId = await PostToUsersAPI(GenerateRandomStr(), GenerateRandomStr(), GenerateRandomStr(),true, true);
            Assert.NotNull(createdId);
            Assert.NotEmpty(createdId);
        }
        [Fact]
        public async Task UsersApi_GET_ShouldWorkCorrectly()
        {
            AddUserRequest request = new() { FullName = GenerateRandomStr(), UserName = GenerateRandomStr(), Password = GenerateRandomStr(), IsAdmin = true, Enabled = true };
            string? response = await PostToUsersAPI(request);
            Assert.NotNull(response);
            int id = Int32.Parse(response);
            List<UserDTO>? getResponse = await GetAsync();
            Assert.NotNull(getResponse);
            Assert.NotEmpty(getResponse);
            Assert.Contains(getResponse, e => e.Id == id);
        }
        [Fact]
        public async Task UsersEmailsApi_DELETE_ShouldWorkCorrectly()
        {
            AddUserRequest request = new() { FullName = GenerateRandomStr(), UserName = GenerateRandomStr(), Password = GenerateRandomStr(), IsAdmin = true, Enabled = true };
            string? createdId = await PostToUsersAPI(request);
            List<UserDTO>? res = await GetAsync();
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
        public async Task LocalMonitoredEmailsApi_PUT_ShouldWorkCorrectly()
        {
            await PostToUsersAPI(GenerateRandomStr(), GenerateRandomStr(), GenerateRandomStr(), true,true);
            await PostToUsersAPI(GenerateRandomStr(), GenerateRandomStr(), GenerateRandomStr(), true, true);
            List<UserDTO>? users = await GetAsync();
            Assert.NotNull(users);
            var updateRequest = new UpdateUserRequest() { Id = users[0].Id, UserName = "Test", FullName = "Test", Enabled = false, IsAdmin =false};
            var response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            response.EnsureSuccessStatusCode();
            users = await GetAsync();
            Assert.NotNull(users);
            Assert.Contains<UserDTO>(users, (UserDTO u) => u.UserName == updateRequest.UserName);
            updateRequest = new UpdateUserRequest() { Id = users.Last().Id, UserName = "Test", FullName = "Test", Enabled = false, IsAdmin = false };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            var code = response.StatusCode;
            Assert.Equal(HttpStatusCode.BadRequest, code);
            updateRequest = new UpdateUserRequest() { Id = -1, UserName = "Test", FullName = "Test", Enabled = false, IsAdmin = false };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            code = response.StatusCode;
            Assert.Equal(HttpStatusCode.BadRequest, code);
            updateRequest = new UpdateUserRequest() { Id = Int32.MaxValue, UserName = "Test", FullName = "Test", Enabled = false, IsAdmin = false };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            code = response.StatusCode;
            Assert.Equal(HttpStatusCode.NotFound, code);
        }
    }
}
