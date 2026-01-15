using CGPGK.Models;
using CGPGK.Services;
using CGPGK.Utils;
using FluentFTP;
using GateKeeper.Core.Application;
using GateKeeper.Core.Context;
using GateKeeper.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static CGPGK.Utils.Utils;

[assembly: InternalsVisibleTo("GateKeeper.Tests")]
namespace CGPGK
{
    internal class CGPGKProgram
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            IConfiguration config = builder.Build();
            var appSettings = config.GetSection("Settings").Get<AppSettings>();
            if (appSettings == null)
            {
                Console.Error.WriteLine("* Unable to read appsettings file.");
                return;
            }
            FTP.GetInstance(appSettings);
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AppSettings>(appSettings)
                .AddSingleton<WorkerService>()
                .AddDbContext<AddressesDbContext>()
                .AddScoped<DatabaseService>()
                .AddScoped<AllowedDomainsApplication>()
                .AddScoped<AllowedEmailsApplication>()
                .AddScoped<ForeingEmailsApplication>()
                .AddScoped<LocalMonitoredEmailsApplication>()
                .BuildServiceProvider();

            var workerService = serviceProvider.GetRequiredService<WorkerService>();
            PrintLogMessage("Free");
            await workerService.Work();
        }
               
    }
}
