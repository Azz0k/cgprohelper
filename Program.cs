using FluentFTP;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CGProToCCAddressHelper.Services;
using CGProToCCAddressHelper.Models;

namespace CGProToCCAddressHelper
{
    internal class Program
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
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AllowedRecipients>()
                .AddSingleton<AppSettings>(appSettings)
                .AddSingleton<UpdateService>()
                .AddSingleton<WorkerService>()
                .BuildServiceProvider();
            var updateService = serviceProvider.GetRequiredService<UpdateService>();
            var workerService = serviceProvider.GetRequiredService<WorkerService>();
            await updateService.UpdateData();
            workerService.Print("* ToCCAddressHelper Free");
            await workerService.Work();
        }
               
    }
}
