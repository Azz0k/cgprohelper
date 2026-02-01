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
using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using static GateKeeper.Core.Utils.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace GateKeeper.Tests
{
    public class BlockedEmailsAPITests :
    IClassFixture<CustomWebApplicationFactory<APIProgram>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<APIProgram>
            _factory;
        private string apiUri = "/api/blockedEmails";
        public BlockedEmailsAPITests(CustomWebApplicationFactory<APIProgram> factory, ITestOutputHelper testOutputHelper)
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
        private async Task SeedData()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var app = scopedServices.GetRequiredService<BlockedEmailsApplication>();
                AddBlockedEmailRequest newEmail = new AddBlockedEmailRequest() { SenderEmail = "test", RecipientEmail = "test", Date = GenerateReceivedDate(), Time = GenerateReceivedTime() };
                await app.AddAsync(newEmail);
            }
        }
        private async Task<List<BlockedEmailsDTO>?> GetAsync()
        {
            var response = await _client.GetAsync(apiUri);
            var content = await response.Content.ReadFromJsonAsync<List<BlockedEmailsDTO>>();
            return content;
        }
        [Fact]
        public async Task BlockedEmailsApi_GET_ShouldWorkCorrectly()
        {
            await SeedData();
            List<BlockedEmailsDTO>? res = await GetAsync();
            Assert.NotNull(res);
            Assert.NotEmpty(res);
            Assert.Equal(GenerateReceivedDate(), res[0].Date);
            Assert.Equal(GenerateReceivedTime(), res[0].Time);
        }
    }
}
