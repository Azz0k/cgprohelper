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
    public class LocalMonitoredEmailsAPITests :
    IClassFixture<CustomWebApplicationFactory<APIProgram>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<APIProgram> _factory;
        private string apiUri = "/api/LocalMonitoredEmails";
        public LocalMonitoredEmailsAPITests(
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
        private string GenerateRandomDomainName()
        {
            return Guid.NewGuid().ToString();
        }
        private async Task<string?> PostToLocalMonitoredEmailsAPI(bool allowed=false)
        {
            string domain = GenerateRandomDomainName();
            LocalMonitoredEmails request = new() { Email = $"test@{domain}", IsReplyAllowed = allowed};
            var response = await _client.PostAsJsonAsync(apiUri, request);
            response.EnsureSuccessStatusCode();
            string? addedData = await response.Content.ReadAsStringAsync();
            return addedData;
        }
        private async Task<List<LocalMonitoredEmailsDTO>?> GetAsync()
        {
            var response = await _client.GetAsync(apiUri);
            var content = await response.Content.ReadFromJsonAsync<List<LocalMonitoredEmailsDTO>>();
            return content;
        }
        [Fact]
        public async Task LocalMonitoredEmailsApi_POST_ShouldWorkCorrectly()
        {
            string? createdId = await PostToLocalMonitoredEmailsAPI();
            Assert.NotNull(createdId);
            Assert.NotEmpty(createdId);
        }
        [Fact]
        public async Task LocalMonitoredEmailsApi_GET_ShouldWorkCorrectly()
        {
            string? postResponceContent = await PostToLocalMonitoredEmailsAPI();
            Assert.NotNull(postResponceContent);
            int notAllowedID = int.Parse(postResponceContent);
            postResponceContent = await PostToLocalMonitoredEmailsAPI(true);
            Assert.NotNull(postResponceContent);
            int allowedID = int.Parse(postResponceContent);
            List<LocalMonitoredEmailsDTO>? getResponceContent = await GetAsync();
            Assert.NotNull(getResponceContent);
            Assert.NotEmpty(getResponceContent);
            Assert.Contains(getResponceContent, e=> e.Id == notAllowedID);
            Assert.True(getResponceContent.Where(e => e.Id == allowedID).All(e=>e.IsReplyAllowed));
            Assert.Contains(getResponceContent, e => e.Id == allowedID);
            Assert.True(getResponceContent.Where(e => e.Id == notAllowedID).All(e => !e.IsReplyAllowed));
        }
        [Fact]
        public async Task LocalMonitoredEmailsApi_DELETE_ShouldWorkCorrectly()
        {
            string? createdId = await PostToLocalMonitoredEmailsAPI();
            List<LocalMonitoredEmailsDTO>? res = await GetAsync();
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
            await PostToLocalMonitoredEmailsAPI();
            await PostToLocalMonitoredEmailsAPI();
            List<LocalMonitoredEmailsDTO>? emails = await GetAsync();
            Assert.NotNull(emails);
            var updateRequest = new UpdateLocalMonitoredEmailsRequest() { Id = emails[0].Id, Email = "update@update.ru" };
            var response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            response.EnsureSuccessStatusCode();
            emails = await GetAsync();
            Assert.NotNull(emails);
            Assert.Contains<LocalMonitoredEmailsDTO>(emails, (LocalMonitoredEmailsDTO e) => e.Email == updateRequest.Email);
            updateRequest = new UpdateLocalMonitoredEmailsRequest() { Id = emails.Last().Id, Email = "update@update.ru" };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            var code = response.StatusCode;
            Assert.Equal(HttpStatusCode.BadRequest, code);
            updateRequest = new UpdateLocalMonitoredEmailsRequest() { Id = -1, Email = "update1@update.ru" };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            code = response.StatusCode;
            Assert.Equal(HttpStatusCode.BadRequest, code);
            updateRequest = new UpdateLocalMonitoredEmailsRequest() { Id = int.MaxValue, Email = "update1@update.ru" };
            response = await _client.PutAsJsonAsync(apiUri, updateRequest);
            code = response.StatusCode;
            Assert.Equal(HttpStatusCode.NotFound, code);
        }
    }
}
