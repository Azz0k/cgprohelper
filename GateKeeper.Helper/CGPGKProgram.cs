using CGPGK.Models;
using CGPGK.Services;
using CGPGK.Utils;
using FluentFTP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
                .AddSingleton<EmailChecker>()
                .AddSingleton<AppSettings>(appSettings)
                .AddSingleton<MonitoredFiles>()
                .AddSingleton<UpdateService>()
                .AddSingleton<WorkerService>()
                .AddSingleton<FileDataStore>()
                .BuildServiceProvider();
            var updateService = serviceProvider.GetRequiredService<UpdateService>();
            var workerService = serviceProvider.GetRequiredService<WorkerService>();
            await updateService.UpdateDataFirstTime();
            workerService.Print("* ToCCAddressHelper Free");
            await workerService.Work();
        }
               
    }
}
