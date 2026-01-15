using CGPGK.Models;
using GateKeeper.Core.Application;
using GateKeeper.Core.Models.Entities;
using GateKeeper.Helper.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CGPGK.Utils.Utils;

namespace CGPGK.Services
{
    internal class WorkerService
    {
        private readonly AppSettings _appSettings;
        IServiceProvider _serviceProvider;
        private CancellationTokenSource cancellationTokenSource = new();
        private Dictionary<FileTypes, MonitoredFileOnFTP> _files = new();
        
        public WorkerService(AppSettings appSettings, IServiceProvider provider)
        {
            _appSettings = appSettings;
            _serviceProvider = provider;
            _files.Add(FileTypes.EmailsFull, new MonitoredFileOnFTP(_appSettings.ConnectionSettings.emailsFullFileName, FileTypes.EmailsFull));
            _files.Add(FileTypes.EmailsDiff, new MonitoredFileOnFTP(_appSettings.ConnectionSettings.emailsDiffFileName, FileTypes.EmailsDiff));
            _files.Add(FileTypes.DomainsFull, new MonitoredFileOnFTP(_appSettings.ConnectionSettings.domainsFullFileName, FileTypes.DomainsFull));
            _files.Add(FileTypes.DomainsDiff, new MonitoredFileOnFTP(_appSettings.ConnectionSettings.domainsDiffFileName, FileTypes.DomainsDiff)); 
        }
        public async Task Work()
        {
            await UpdateDataFirstTime();
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                string? line = await Console.In.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                _ = Task.Run(() =>
                {
                    //ProcessMessage(line);
                });
            }
        }
        private async Task UpdateDataFirstTime()
        {
            try
            {
                await CheckAllFilesAsync();
            }
            catch (Exception ex)
            {
                PrintLogMessage(ex.Message);
            }
            _ = Task.Run(async () => { await BackGroundLoop(); }, cancellationTokenSource.Token);

        }
        private async Task BackGroundLoop()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(1000 * _appSettings.updateIntervalInSeconds, cancellationTokenSource.Token);
                await CheckAllFilesAsync();
            }
        }
        private async Task CheckAllFilesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var app = scope.ServiceProvider.GetRequiredService<HelperApplication>();
            foreach (var pairs in _files)
            {
                FileTypes fileType = pairs.Key;
                MonitoredFileOnFTP file = pairs.Value;
                List<string> data = await file.ReadAllLinesIfChangedAsync();
                if (data.Count > 0)
                {
                    await app.UpdateDataFromFTPAsync(fileType, data);
                    PrintLogMessage($"{fileType} updated");
                }

            }
        }
    }
}
