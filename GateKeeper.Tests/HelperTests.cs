using CGPGK.Services;
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
using System.Runtime.InteropServices.Marshalling;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace GateKeeper.Tests
{
    public class HelperTests :
    IClassFixture<HelperWebApplicationFactory<APIProgram>>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HelperWebApplicationFactory<APIProgram> _factory;
        private UpdateService updateService;
        private WorkerService workerService;
        public HelperTests(HelperWebApplicationFactory<APIProgram> factory, ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            _testOutputHelper = testOutputHelper;
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                updateService = scopedServices.GetRequiredService<UpdateService>();
                workerService = scopedServices.GetRequiredService<WorkerService>();
            }
        }
        [Fact]
        public async Task Helper_ShouldWorkCorrectly()
        {
            await updateService.UpdateDataFirstTime();
        }
    }
}
