using CGPGK.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CGPGK.Services
{
    internal class UpdateService
    {
        private EmailChecker _allowedRecipients;
        private readonly AppSettings _appSettings;
        private MonitoredFiles monitoredFiles;
        private CancellationTokenSource updateSource = new CancellationTokenSource();
        private int updateInterval = 60;

        public UpdateService(AppSettings appSettings, EmailChecker allowedRecipients, MonitoredFiles monitoredFiles)
        {
            _appSettings = appSettings;
            _allowedRecipients = allowedRecipients;
            string currentDir = _appSettings.currentDir;
            if (_appSettings.updateIntervalInSeconds > 0)
            {
                updateInterval = _appSettings.updateIntervalInSeconds;  
            }
            this.monitoredFiles = monitoredFiles;
        }

        public async Task UpdateDataFirstTime()
        {
            try
            {
                await monitoredFiles.CheckAllFilesAsync();
            }
            catch (Exception ex)
            {
                WriteErrorAndExit(ex.Message);
            }
            updateSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var backgroundTask = Task.Run(() => { BackGroundLoop(); }, updateSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task BackGroundLoop()
        {
            while (!updateSource.Token.IsCancellationRequested)
            {
                await Task.Delay(1000 * updateInterval, updateSource.Token);
                while (!_allowedRecipients.isUpdateAllowed || updateSource.Token.IsCancellationRequested)
                {
                }
                await monitoredFiles.CheckAllFilesAsync();
            }
        }

        
        private void WriteErrorAndExit(string message="")
        {
            updateSource.Cancel();
            Console.Error.WriteLine("* Error: The address files not found");
            if (message != "") 
                Console.Error.WriteLine($"* {message}");
            Environment.Exit(1);
        }
    }

}
