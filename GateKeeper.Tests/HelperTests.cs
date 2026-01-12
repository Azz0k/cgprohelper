using CGPGK.Models;
using CGPGK.Services;
using CGPGK.Utils;
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

        private UpdateService updateService;
        private WorkerService workerService;
        public HelperTests()
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"HelperAppSettings.json");
            IConfiguration config = builder.Build();
            var appSettings = config.GetSection("Settings").Get<AppSettings>();
            Assert.NotNull(appSettings);
            FTP.GetInstance(appSettings);
            var serviceProvider = new ServiceCollection()
                .AddSingleton<EmailChecker>()
                .AddSingleton<AppSettings>(appSettings)
                .AddSingleton<MonitoredFiles>()
                .AddSingleton<UpdateService>()
                .AddSingleton<WorkerService>()
                .AddSingleton<FileDataStore>()
                .BuildServiceProvider();
            updateService = serviceProvider.GetRequiredService<UpdateService>();
            workerService = serviceProvider.GetRequiredService<WorkerService>();
        }
        [Fact]
        public async Task Helper_ShouldWorkCorrectly()
        {
            await updateService.UpdateDataFirstTime();
        }
    }
    
}
