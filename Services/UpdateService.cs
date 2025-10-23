using CGProToCCAddressHelper.Models;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CGProToCCAddressHelper.Services
{
    internal class UpdateService
    {
        private AllowedRecipients _allowedRecipients;
        private readonly AppSettings _appSettings;
        private string recipientsFile;
        private CancellationTokenSource updateSource = new CancellationTokenSource();
        private FtpService _ftpService;
        private int updateInterval = 60;
        public UpdateService(AppSettings appSettings, AllowedRecipients allowedRecipients, FtpService ftpService)
        {
            _appSettings = appSettings;
            _allowedRecipients = allowedRecipients;
            string currentDir = _appSettings.currentDir;
            string fileName = _appSettings.emailsLocalFullFileName;
            recipientsFile = Path.Combine(currentDir, fileName);
            _ftpService = ftpService;
            if (_appSettings.updateIntervalInSeconds > 0)
            {
                updateInterval = _appSettings.updateIntervalInSeconds;  
            }
        }

        public async Task UpdateDataFirstTime()
        {
            try
            {
                _allowedRecipients.UpdateRecipients(await _ftpService.DownloadFullFileAsync());
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
                bool needUpdateEmailsFullListFile = await _ftpService.DownloadIfNeededAsync(updateSource.Token);
                if (needUpdateEmailsFullListFile)
                {
                    while (!_allowedRecipients.isUpdateAllowed || updateSource.Token.IsCancellationRequested)
                    {
                    }
                    _allowedRecipients.UpdateRecipients(await _ftpService.DownloadFullFileAsync());
                }
            }
        }

        private void GetFromFTP()
        {

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
